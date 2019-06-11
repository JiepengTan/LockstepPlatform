using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Server.Common;
using Lockstep.Util;
using NetMsg.Common;
using NetMsg.Server;
using Room = Server.Servers.Lobby.LobbyServer.Room;
using User = Server.Servers.Lobby.LobbyServer.User;

namespace Lockstep.Server.Lobby {
    public partial class LobbyServer : Common.Server {
        public enum ELobbyBorderType {
            InRoom,
            OutRoom,
            All
        }

        //Server DS
        private NetServer<EMsgLS> _netServerLS;
        private NetServer<EMsgSC> _netServerSC;

        //room
        private Dictionary<int, List<Room>> _gameType2Rooms = new Dictionary<int, List<Room>>();

        private Dictionary<int, Room> _roomId2Room = new Dictionary<int, Room>();

        //player  
        private Dictionary<long, User> _uid2Player = new Dictionary<long, User>();
        private Dictionary<int, User> _peerId2Player = new Dictionary<int, User>();

        private int _roomIdCounter;

        private Dictionary<int, List<RoomInfo>> _type2RroomInfos = new Dictionary<int, List<RoomInfo>>();
        private Dictionary<int, RoomInfo[]> _cachedType2RoomInfos = new Dictionary<int, RoomInfo[]>();
        private Dictionary<int, IPeer> _peerId2Servers = new Dictionary<int, IPeer>();
        private List<IPeer> _allGameServers = new List<IPeer>();

        public override void DoStart(){
            base.DoStart();
            InitNetServer(ref _netServerLS, _serverConfig.serverPort, OnServerDisconnected);
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort, OnPlayerDisconnected);
        }

        void OnServerDisconnected(IPeer peer){
            Debug.Assert(peer != null);
            if (_peerId2Servers.TryGetValue(peer.Id, out var user)) {
                _allGameServers.Remove(peer);
            }
        }

        void OnPlayerDisconnected(IPeer peer){
            Debug.Assert(peer != null);
            if (_peerId2Player.TryGetValue(peer.Id, out var user)) {
                RemovePlayer(user);
            }
        }

        #region Status

        private RoomInfo[] GetRoomInfos(int gameType){
            if (_cachedType2RoomInfos.TryGetValue(gameType, out var infos)) {
                return infos;
            }
            else {
                var roomInfos = _type2RroomInfos.GetRefVal(gameType);
                if (roomInfos == null || roomInfos.Count == 0) {
                    return null;
                }
                else {
                    const int MaxRoomCachedCount = 200;
                    //限制最大数量
                    RoomInfo[] retInfos = new RoomInfo[MaxRoomCachedCount];
                    if (roomInfos.Count > MaxRoomCachedCount) {
                        for (int i = 0; i < MaxRoomCachedCount; i++) {
                            retInfos[i] = roomInfos[i];
                        }
                    }
                    else {
                        retInfos = roomInfos.ToArray();
                    }

                    _cachedType2RoomInfos.Add(gameType, retInfos);
                    return retInfos;
                }
            }
        }

        private void AddPlayer(long userId, int gameType, string name, string account, string loginHash){
            var player = Pool.Get<User>();
            player.Init(userId, gameType, name, account, loginHash);
            _uid2Player[userId] =  player;
            Debug.Log("AddPlayer " + player.Account);
        }

        private void RemovePlayer(User user, bool isForce = false){
            Debug.Log("RemovePlayer " + user.Account + " id " +  user.UserId);
            var room = user.Room;

            if (room != null) {
                if (room.IsPlaying && !isForce) { //游戏中的玩家需要考虑断线重连
                    user.Room = null;
                    _peerId2Player.Remove(user.Peer.Id);
                    return;
                }
                room.ServerPeer?.SendMessage((short) EMsgLS.L2G_UserLeave, new Msg_L2G_UserLeave() {
                    RoomId = room.RoomId,
                    UserId = user.UserId
                });
                room.RemoveUser(user);
                if (room.IsEmpty) {
                    RemoveRoom(room);
                }
            }

            user.Room = null;
            _uid2Player.Remove(user.UserId);
            _peerId2Player.Remove(user.Peer.Id);
            user.Peer.CleanExtension();
            user.Peer?.Disconnect("Tick out");
            user.Peer = null;
        }


        private Room AddRoom(User owner, string name, int mapId, byte size){
            var room = Pool.Get<Room>();
            var gameType = owner.GameType;
            owner.Room = room;
            room.Init(gameType, _roomIdCounter++, name, mapId, size, owner);
            _roomId2Room.Add(room.RoomId, room);
            if (_gameType2Rooms.TryGetValue(gameType, out var prooms)) {
                prooms.Add(room);
            }
            else {
                var rooms = new List<Room>();
                rooms.Add(room);
                _gameType2Rooms.Add(gameType, rooms);
            }

            if (_type2RroomInfos.TryGetValue(gameType, out var pRoomInfos)) {
                pRoomInfos.Add(room.Info);
            }
            else {
                var roomInfos = new List<RoomInfo>();
                roomInfos.Add(room.Info);
                _type2RroomInfos.Add(gameType, roomInfos);
            }

            _cachedType2RoomInfos.Remove(gameType);
            return room;
        }

        private void RemoveRoom(Room room){
            Debug.Log("RemoveRoom " + room.RoomId);
            Debug.Assert(room.CurPlayerCount == 0);
            _roomId2Room.Remove(room.RoomId);
            if (_gameType2Rooms.TryGetValue(room.GameType, out var lst)) {
                lst.Remove(room);
                if (lst.Count == 0) {
                    _gameType2Rooms.Remove(room.GameType);
                }
            }

            if (_type2RroomInfos.TryGetValue(room.GameType, out var pInfolst)) {
                pInfolst.Remove(room.Info);
                if (lst.Count == 0) {
                    _gameType2Rooms.Remove(room.GameType);
                }
            }

            _cachedType2RoomInfos.Remove(room.GameType);

            Pool.Return(room);
        }

        #endregion


        #region msgs

        protected void C2L_ReqRoomList(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_ReqRoomList>();
            var user = reader.Peer.GetExtension<User>();
            var startIdx = msg.StartIdx;
            if (user != null) {
                var infos = GetRoomInfos(user.GameType);
                user.SendMessage((short) EMsgSC.L2C_RoomList, new Msg_L2C_RoomList() {
                    Rooms = infos
                });
            }
        }

        protected void G2L_RegisterServer(IIncommingMessage reader){
            var msg = reader.Parse<Msg_RegisterServer>();
            if (!_peerId2Servers.ContainsKey(reader.Peer.Id)) {
                _peerId2Servers[reader.Peer.Id] = reader.Peer;
                reader.Peer.AddExtension(msg.ServerInfo);
                _allGameServers.Add(reader.Peer);
            }
        }


        protected void I2L_UserLogin(IIncommingMessage reader){
            var msg = reader.Parse<Msg_I2L_UserLogin>();
            if (_uid2Player.TryGetValue(msg.UserId, out var oldPlayer)) {
                oldPlayer.LoginHash = msg.LoginHash;//避免重连
                //Tick out 
                Debug.Log(oldPlayer.Account + " login at another place  id" + msg.UserId);
                oldPlayer.SendMessage((short)EMsgSC.S2C_TickPlayer,new Msg_S2C_TickPlayer() {
                    Reason =  1
                });
                CoroutineHelper.StartCoroutine(LoginAndTickPlayer(reader, oldPlayer, msg));
            }
            else { 
                AddPlayer(msg.UserId, msg.GameType, msg.Account, msg.Account, msg.LoginHash);
                reader.Respond(1, EResponseStatus.Success);
            }
        }

        private IEnumerator LoginAndTickPlayer(IIncommingMessage reader, User oldPlayer, Msg_I2L_UserLogin msg){
            Debug.Log("before WaitRemovePlayer " + Time.timeSinceLevelLoad);
            yield return new WaitForSeconds(0.5f);
            Debug.Log("after WaitRemovePlayer "+ Time.timeSinceLevelLoad);
            RemovePlayer(oldPlayer,true);
            AddPlayer(msg.UserId, msg.GameType, msg.Account, msg.Account, msg.LoginHash);
            reader.Respond(1, EResponseStatus.Success);
        }


        protected void C2L_UserLogin(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_UserLogin>();
            var uid = msg.userId;
            if (_uid2Player.TryGetValue(uid, out var user)) {
                if (user.LoginHash == msg.LoginHash) {
                    var isReconnected = user.Room != null && user.Room.IsPlaying;
                    Debug.Log("UserLogin " + msg + " isReconnect: " + isReconnected);
                    //断线重连
                    if (isReconnected) {
                        Debug.Log("TODO Need deal reconnect");
                        return;
                    }
                    else {
                        user.Peer = reader.Peer;
                        _peerId2Player[user.Peer.Id] = user;
                        reader.Peer.AddExtension(user);
                        var roomInfos = GetRoomInfos(user.GameType);
                        reader.Respond(EMsgSC.L2C_RoomList, new Msg_L2C_RoomList() {
                            GameType = user.GameType,
                            Rooms = roomInfos
                        });
                    }
                }
                else {
                    reader.Respond((int) ELoginResult.NotLogin, EResponseStatus.Failed);
                }
            }
            else {
                reader.Respond((int) ELoginResult.NotLogin, EResponseStatus.Failed);
            }
        }

        protected void C2L_CreateRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_CreateRoom>();
            var user = reader.Peer.GetExtension<User>();
            if (user?.Room != null) {
                reader.Respond(1, EResponseStatus.Failed);
                return;
            }

            Debug.Log("AddRoom " + msg);
            var room = AddRoom(user, msg.Name, msg.MapId, msg.MaxPlayerCount);
            reader.Respond(EMsgSC.L2C_CreateRoomResult, new Msg_L2C_CreateRoom() {
                Info = room.Info,
                PlayerInfos = room.RoomPlayerInfos
            });
            var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate, new Msg_L2C_RoomInfoUpdate() {
                AddInfo = new RoomInfo[] {room.Info}
            });
            BorderMessage(bMsg, user.GameType, ELobbyBorderType.OutRoom);
        }


        protected void C2L_JoinRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_JoinRoom>();
            Debug.Log("C2L_JoinRoom" + msg);
            var roomId = msg.RoomId;
            if (_roomId2Room.TryGetValue(roomId, out var room)) {
                if (room.IsFull||room.IsPlaying) {
                    reader.Respond((int) ERoomOperatorResult.Full, EResponseStatus.Failed);
                    return;
                }
                else {
                    var user = reader.Peer.GetExtension<User>();
                    if (user?.Room == null) {
                        user.Room = room;
                        room.AddUser(user);
                        reader.Respond((short) EMsgSC.L2C_JoinRoomResult, new Msg_L2C_JoinRoomResult() {
                            PlayerInfos = room.RoomPlayerInfos
                        });
                        room.BorderMessage((short) EMsgSC.L2C_JoinRoom, new Msg_L2C_JoinRoom() {
                            PlayerInfo = new RoomPlayerInfo() {
                                UserId = user.UserId,
                                Name = user.Name,
                                Status = user.IsReady
                            }
                        });

                        var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate, new Msg_L2C_RoomInfoUpdate() {
                            ChangedInfo = new RoomChangedInfo[]
                                {new RoomChangedInfo() {RoomId = room.RoomId, CurPlayerCount = room.CurPlayerCount}}
                        });
                        BorderMessage(bMsg, user.GameType, ELobbyBorderType.OutRoom);
                    }
                    else {
                        reader.Respond((int) ERoomOperatorResult.AlreadyExist, EResponseStatus.Failed);
                    }
                }
            }
            else {
                reader.Respond((int) ERoomOperatorResult.NotExist, EResponseStatus.Failed);
            }
        }

        protected void C2L_LeaveRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_LeaveRoom>();
            var user = reader.Peer.GetExtension<User>();
            if (user?.Room == null) {
                return;
            }

            var room = user.Room;
            room.RemoveUser(user);
            if (room.IsEmpty) {
                var roomId = room.RoomId;
                user.Room = null;
                //TODO 缓存      
                var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate, new Msg_L2C_RoomInfoUpdate() {
                    DeleteInfo = new[] {roomId}
                });
                BorderMessage(bMsg, user.GameType, ELobbyBorderType.OutRoom);

                RemoveRoom(room);
                reader.Respond((int) ERoomOperatorResult.Succ, EResponseStatus.Success);
                return;
            }

            if (room.Owner == user) {
                room.Owner = room.Users[0];
            }

            user.Room = null;
            reader.Respond((int) ERoomOperatorResult.Succ, EResponseStatus.Success);
            room.BorderMessage((short) EMsgSC.L2C_LeaveRoom, new Msg_L2C_LeaveRoom() {
                UserId = user.UserId
            });
            {
                var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate, new Msg_L2C_RoomInfoUpdate() {
                    ChangedInfo = new RoomChangedInfo[]
                        {new RoomChangedInfo() {RoomId = room.RoomId, CurPlayerCount = room.CurPlayerCount}}
                });
                BorderMessage(bMsg, user.GameType, ELobbyBorderType.OutRoom);
            }
        }


        protected void C2L_ReadyInRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_ReadyInRoom>();
            var user = reader.Peer.GetExtension<User>();
            if(user == null) return;
            var uid = user.UserId;
            if (_uid2Player.TryGetValue(uid, out var info)) {
                if (info.IsReady != msg.State && user.Room != null && !user.Room.IsPlaying) {
                    info.IsReady = msg.State;
                    user.Room.BorderMessage((short) EMsgSC.L2C_ReadyInRoom, new Msg_L2C_ReadyInRoom() {
                        UserId = uid,
                        State = msg.State
                    });
                }
            }
        }


        protected void C2L_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_StartGame>();
            Debug.Log("C2L_StartGame" + msg);
            var user = reader.Peer.GetExtension<User>();
            if(user == null) return;
            var room = user.Room;
            if (room?.Owner != user) {
                reader.Respond(1, EResponseStatus.Failed);
                return;
            }

            if (room.IsPlaying || _allGameServers.Count <= 0) {
                reader.Respond(room.IsPlaying ? 2 : 3, EResponseStatus.Failed);
                return;
            }

            var gameHash = "game" + LRandom.Next();
            var playerInfos = new GamePlayerInfo[room.Users.Count];
            for (int i = 0; i < room.Users.Count; i++) {
                var pUser = room.Users[i];
                playerInfos[i] = new GamePlayerInfo() {
                    UserId = pUser.UserId,
                    Account = pUser.Account,
                    LoginHash = pUser.LoginHash,
                };
            }

            //TODO 使用平衡策略去获取服务器
            var server = _allGameServers[LRandom.Next(_allGameServers.Count)];
            server?.SendMessage((short) EMsgLS.L2G_CreateRoom, new Msg_L2G_CreateRoom() {
                    GameType = user.GameType,
                    Players = playerInfos,
                    MapId = room.MapId,
                    GameHash = gameHash
                }, (status, response) => {
                    if (status != EResponseStatus.Failed) {
                        var ipInfo = server.GetExtension<ServerIpInfo>();
                        room.ServerPeer = server;
                        room.IsPlaying = true;
                        var retMsg = new Msg_L2C_StartGame() {
                            GameServerEnd = new IPEndInfo() {
                                Ip = ipInfo.Ip,
                                Port = ipInfo.Port
                            },
                            GameHash = gameHash,
                            RoomId = response.AsInt()
                        };
                        foreach (var roomUser in room.Users) {
                            roomUser.Peer?.SendMessage((short) EMsgSC.L2C_StartGame, retMsg);
                        }
                    }
                }
            );
        }

        protected void G2L_OnGameFinished(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2L_OnGameFinished>();
            var roomId = msg.RoomId;
            if (_roomId2Room.TryGetValue(roomId, out var room)) {
                Debug.Log("OnGameFinished " + room.RoomId);
                foreach (var user in room.Users.ToArray()) {
                    room.RemoveUser(user);
                    user.Room = null;
                }

                var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate, new Msg_L2C_RoomInfoUpdate() {
                    DeleteInfo = new int[] {room.RoomId}
                });
                BorderMessage(bMsg, room.GameType, ELobbyBorderType.OutRoom);
                RemoveRoom(room);
            }
            else {
                Debug.LogError("Room Finished but room destroyed? " + roomId);
            }
        }


        protected void C2L_RoomChatInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_RoomChatInfo>();
            Debug.Log("C2L_RoomChatInfo" + msg);
            var user = reader.Peer.GetExtension<User>();
            user?.Room?.BorderMessage((short) EMsgSC.L2C_RoomChatInfo, new Msg_L2C_RoomChatInfo() {
                ChatInfo = msg.ChatInfo
            });
        }

        private void BorderMessage(IMessage msg, int gameType, ELobbyBorderType type){
            switch (type) {
                case ELobbyBorderType.InRoom:
                    if (_gameType2Rooms.TryGetValue(gameType, out var rooms)) {
                        foreach (var tRoom in rooms) {
                            tRoom.BorderMessage(msg);
                        }
                    }

                    break;
                case ELobbyBorderType.OutRoom:
                    foreach (var pair in _peerId2Player) {
                        var user = pair.Value;
                        if (user != null && user.Room == null) {
                            user.SendMessage(msg);
                        }
                    }

                    break;
                case ELobbyBorderType.All:
                    foreach (var pair in _peerId2Player) {
                        var user = pair.Value;
                        user?.SendMessage(msg);
                    }

                    break;
            }
        }

        #endregion
    }
}