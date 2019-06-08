using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Xml;
using LiteDB;
using LiteNetLib;
using LitJson;
using Lockstep.Serialization;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Server.Common;
using Lockstep.Server.Database;
using Lockstep.Util;
using NetMsg.Common;
using NetMsg.Server;
using IRecyclable = Lockstep.Util.IRecyclable;

namespace Lockstep.Server.Lobby {
    public interface ILobbyServer : IServer {
        //room operator
        List<IRoom> GetRooms(int roomType);
        IRoom GetRoom(int roomId);
        IRoom GetRoomByUserID(int id);
        IRoom CreateRoom(int type, Player master, string roomName, byte size);
        void RemoveRoom(IRoom room);
        bool JoinRoom(Player player, int roomID);
        bool LeaveRoom(Player player);

        //players
        void TickOut(Player player, int reason);
        Player GetPlayer(long playerId);

        //Net status
        void OnClientConnected(object peer);

        void OnCilentDisconnected(object peer);
        //msg handle 
    }

    public class LobbyServer : Common.Server {
        public class Room : BaseRecyclable {
            public RoomInfo Info = new RoomInfo();

            public bool IsPlaying {
                get => Info.State == 1;
                set => Info.State = (byte) (value ? 1 : 0);
            }

            public int GameType {
                get => Info.GameType;
                set => Info.GameType = value;
            }

            public int RoomId {
                get => Info.RoomId;
                set => Info.RoomId = value;
            }

            public int MapId {
                get => Info.MapId;
                set => Info.MapId = value;
            }

            public string Name {
                get => Info.Name;
                set => Info.Name = value;
            }

            public byte CurPlayerCount {
                get => Info.CurPlayerCount;
                set => Info.CurPlayerCount = value;
            }

            public byte MaxPlayerCount {
                get => Info.MaxPlayerCount;
                set => Info.MaxPlayerCount = value;
            }

            public string OwnerName {
                get => Info.OwnerName;
                set => Info.OwnerName = value;
            }

            public bool IsFull => CurPlayerCount >= MaxPlayerCount;

            public bool IsEmpty => CurPlayerCount <= 0;
            public List<User> Users = new List<User>(10);
            public User Owner;
            private RoomPlayerInfo[] _roomPlayerInfos;

            public RoomPlayerInfo[] RoomPlayerInfos {
                get {
                    if (_roomPlayerInfos != null)
                        return _roomPlayerInfos;
                    _roomPlayerInfos = new RoomPlayerInfo[Users.Count];
                    for (int i = 0; i < Users.Count; i++) {
                        var user = Users[i];
                        var info = new RoomPlayerInfo() {
                            UserId = user.UserId,
                            Name = user.Name,
                            Status = user.IsReady
                        };
                        _roomPlayerInfos[i] = info;
                    }

                    return _roomPlayerInfos;
                }
            }


            public void BorderMessage(short opCode, ISerializablePacket packet){
                var msg = MessageHelper.Create(opCode, packet.ToBytes());
                foreach (var user in Users) {
                    user.SendMessage(msg);
                }
            }

            public void BorderMessage(IMessage msg){
                foreach (var user in Users) {
                    user.SendMessage(msg);
                }
            }

            public void Init(int type, int roomId, string name, int mapId, byte maxPlayerCount, User owner){
                GameType = type;
                RoomId = roomId;
                Name = name;
                MapId = mapId;
                MaxPlayerCount = maxPlayerCount;

                CurPlayerCount = 1;
                OwnerName = owner.Name;
                IsPlaying = false;
                Users.Add(owner);
                Owner = owner;
            }

            public override void OnRecycle(){
                Users.Clear();
                Owner = null;
                _roomPlayerInfos = null;
            }

            public void AddUser(User user){
                Users.Add(user);
                CurPlayerCount++;
                _roomPlayerInfos = null;
            }

            public void RemoveUser(User user){
                if (Users.Remove(user)) {
                    CurPlayerCount--;
                    _roomPlayerInfos = null;
                }
            }

            public override string ToString(){
                return JsonMapper.ToJson(this);
            }
        }

        public class User : BaseRecyclable {
            public int GameType;
            public long UserId;
            public string Name;
            public byte IsReady;
            public string Account;
            public string LoginHash; //用于校验玩家
            public IPeer Peer;
            public Room Room;

            public void Init(long userID, int gameType, string name, string account, string loginHash){
                this.UserId = userID;
                this.GameType = gameType;
                this.Name = name;
                this.Account = account;
                this.LoginHash = loginHash;
            }

            public override void OnRecycle(){
                Room = null;
                Peer = null;
            }

            public override string ToString(){
                return JsonMapper.ToJson(this);
            }

            public void SendMessage(short opCode, ISerializablePacket packet){
                Peer?.SendMessage(opCode, packet);
            }

            public void SendMessage(IMessage message){
                Peer?.SendMessage(message);
            }

            public void SendMessage(IMessage message, ResponseCallback responseCallback){
                Peer?.SendMessage(message, responseCallback);
            }
        }

        //room
        private Dictionary<int, List<Room>> _gameType2Rooms = new Dictionary<int, List<Room>>();

        private Dictionary<int, Room> _roomId2Room = new Dictionary<int, Room>();

        //player  
        private Dictionary<long, User> _uid2Player = new Dictionary<long, User>();

        private int _RoomIdCounter;

        public override void DoStart(){
            base.DoStart();
            InitNetInfo();
        }

        #region Status

        private Dictionary<int, List<RoomInfo>> _type2RroomInfos = new Dictionary<int, List<RoomInfo>>();
        private Dictionary<int, RoomInfo[]> _cachedType2RoomInfos = new Dictionary<int, RoomInfo[]>();

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
                    var retInfos = roomInfos.ToArray();
                    _cachedType2RoomInfos.Add(gameType, retInfos);
                    return retInfos;
                }
            }
        }

        private List<Room> GetRooms(int gameType){
            return _gameType2Rooms.GetRefVal(gameType);
        }

        private Room GetRoom(int roomId){
            return _roomId2Room.GetRefVal(roomId);
        }

        private void OnPlayerLogin(long userID, int gameType, string name, string account, string loginHash){
            if (_uid2Player.TryGetValue(userID, out var oldPlayer)) {
                oldPlayer.LoginHash = loginHash;
                oldPlayer.GameType = gameType;
            }
            else {
                var player = Pool.Get<User>();
                player.Init(userID, gameType, name, account, loginHash);
                _uid2Player.Add(userID, player);
            }
        }

        private void RemovePlayer(User user){
            user.Room?.RemoveUser(user);
            user.Room = null;
            _uid2Player.Remove(user.UserId);
        }

        private Room AddRoom(User owner, string name, int mapId, byte size){
            var room = Pool.Get<Room>();
            var gameType = owner.GameType;
            owner.Room = room;
            room.Init(gameType, _RoomIdCounter++, name, mapId, size, owner);
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


        #region Server Net Info

        //Server DS
        private NetServer<EMsgLS> _netServerLS;
        private NetServer<EMsgSC> _netServerSC;

        void InitNetInfo(){
            InitServerLS();
            InitServerSC();
        }

        private void InitServerLS(){
            InitNetServer(ref _netServerLS, _serverConfig.serverPort);
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort);
        }

        #endregion

        #region msgs

        public Dictionary<int, IPeer> _peerId2Servers = new Dictionary<int, IPeer>();
        public List<IPeer> _allGameServers = new List<IPeer>();

        protected void I2L_UserLogin(IIncommingMessage reader){
            var msg = reader.Parse<Msg_I2L_UserLogin>();
            Debug.Log("I2L_UserLogin" + msg);
            OnPlayerLogin(msg.UserId, msg.GameType, msg.Account, msg.Account, msg.LoginHash);
            reader.Respond(1, EResponseStatus.Success);
        }

        protected void G2L_RegisterServer(IIncommingMessage reader){
            var msg = reader.Parse<Msg_RegisterServer>();
            Debug.Log("I2L_UserLogin" + msg);
            if (!_peerId2Servers.ContainsKey(reader.Peer.Id)) {
                _peerId2Servers[reader.Peer.Id] = reader.Peer;
                reader.Peer.AddExtension(msg.ServerInfo);
                _allGameServers.Add(reader.Peer);
            }
        }

        protected void C2L_ReadyInRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_ReadyInRoom>();
            var user = reader.Peer.GetExtension<User>();
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

        protected void C2L_UserLogin(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_UserLogin>();
            Debug.Log("C2L_UserLogin" + msg);
            var uid = msg.userId;
            if (_uid2Player.TryGetValue(uid, out var info)) {
                if (info.LoginHash == msg.LoginHash) {
                    info.Peer = reader.Peer;
                    reader.Peer.AddExtension(info);
                    var roomInfos = GetRoomInfos(info.GameType);
                    reader.Respond(EMsgSC.L2C_RoomList, new Msg_L2C_RoomList() {
                        GameType = info.GameType,
                        Rooms = roomInfos
                    });
                }
                else {
                    reader.Respond((int) ELoginResult.NotLogin, EResponseStatus.Failed);
                }
            }
            else {
                reader.Respond((int) ELoginResult.NotLogin, EResponseStatus.Failed);
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

        protected void C2L_JoinRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_JoinRoom>();
            Debug.Log("C2L_JoinRoom" + msg);
            var roomId = msg.RoomId;
            if (_roomId2Room.TryGetValue(roomId, out var room)) {
                if (room.IsFull) {
                    reader.Respond((int) ERoomOperatorResult.Full, EResponseStatus.Failed);
                    return;
                }
                else {
                    var user = reader.Peer.GetExtension<User>();
                    if (user.Room == null) {
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
                var roomId = user.Room.RoomId;
                //TODO 缓存
                if (_gameType2Rooms.TryGetValue(user.GameType, out var rooms)) {
                    var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate,
                        new Msg_L2C_RoomInfoUpdate() {
                            DeleteInfo = new[] {roomId}
                        }.ToBytes());
                    foreach (var tRoom in rooms) {
                        tRoom.BorderMessage(bMsg);
                    }
                }

                RemoveRoom(user.Room);
                reader.Respond((int) ERoomOperatorResult.Succ, EResponseStatus.Success);
                user.Room = null;
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
        }

        protected void C2L_CreateRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_CreateRoom>();
            var user = reader.Peer.GetExtension<User>();
            if (user?.Room != null) {
                reader.Respond(1, EResponseStatus.Failed);
                return;
            }

            var room = AddRoom(user, msg.Name, msg.MapId, msg.MaxPlayerCount);
            reader.Respond(EMsgSC.L2C_CreateRoomResult, new Msg_L2C_CreateRoom() {
                Info = room.Info,
                PlayerInfos = room.RoomPlayerInfos
            });
            //TODO 缓存信息 批量发送
            if (_gameType2Rooms.TryGetValue(user.GameType, out var rooms)) {
                var bMsg = MessageHelper.Create((short) EMsgSC.L2C_RoomInfoUpdate,
                    new Msg_L2C_RoomInfoUpdate() {
                        AddInfo = new RoomInfo[] {room.Info}
                    }.ToBytes());
                foreach (var tRoom in rooms) {
                    tRoom.BorderMessage(bMsg);
                }
            }
        }


        protected void C2L_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_StartGame>();
            Debug.Log("C2L_StartGame" + msg);
            var user = reader.Peer.GetExtension<User>();
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
                var puser = room.Users[i];
                playerInfos[i] = new GamePlayerInfo() {
                    UserId = puser.UserId,
                    Account = puser.Account,
                    LoginHash = puser.LoginHash,
                };
            }

            var server = _allGameServers[LRandom.Next(_allGameServers.Count)];
            server.SendMessage((short) EMsgLS.L2G_StartGame, new Msg_L2G_StartGame() {
                    GameType = user.GameType,
                    Players = playerInfos,
                    MapId = room.MapId,
                    GameHash = gameHash
                }, (status, response) => {
                    if (status != EResponseStatus.Failed) {
                        var ipInfo = server.GetExtension<ServerIpInfo>();
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

        #endregion
    }
}