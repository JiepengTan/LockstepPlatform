using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using Lockstep.Logging;
using NetMsg.Server;
using Lockstep.Server.Common;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public class GameServer : Common.Server, IGameServer {
        private NetServer<EMsgSC> _netServerSCUdp;
        private NetServer<EMsgSC> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;
        protected NetClient<EMsgLS> _netClientLG; //其他类型的Master 用于提供服务

        private Dictionary<int, BaseRoom> _roomId2Rooms = new Dictionary<int, BaseRoom>();

        private Dictionary<long, Player> _uid2Players = new Dictionary<long, Player>();

        private int CurRoomId;

        private BaseRoom[] _cachedBaseRooms;

        public IPEndInfo TcpEnd { get; set; }
        public IPEndInfo UdpEnd { get; set; }

        private BaseRoom[] CachedBaseRooms {
            get {
                if (_cachedBaseRooms == null) {
                    _cachedBaseRooms = _roomId2Rooms.Values.ToArray();
                }

                return _cachedBaseRooms;
            }
        }

        public override void DoStart(){
            base.DoStart();
            InitServerSC();
            TcpEnd = new IPEndInfo() {Ip = Ip, Port = _serverConfig.tcpPort};
            UdpEnd = new IPEndInfo() {Ip = Ip, Port = _serverConfig.udpPort};
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            var rooms = CachedBaseRooms;
            foreach (var room in rooms) {
                room.DoUpdate(deltaTime);
            }
        }

        public void TickOut(Player player, int reason){ }

        public void OnGameEmpty(IRoom game){
            RemoveGame(game);
        }

        public void OnGameFinished(IRoom game){
            Log($" OnGameFinished " + game.RoomId);
            RemoveGame(game);
        }

        void RemoveGame(IRoom game){
            if (_roomId2Rooms.TryGetValue(game.RoomId, out var room)) {
                _netClientLG.SendMessage(EMsgLS.G2L_OnGameFinished, new Msg_G2L_OnGameFinished() {
                    RoomId = game.RoomId
                });
                room.IsFinished = true;
                foreach (var uid in game.UserIds) {
                    _uid2Players.Remove(uid);
                }

                _roomId2Rooms.Remove(game.RoomId);
                _cachedBaseRooms = null;
            }
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort, OnPlayerDisconnected);
            InitNetServer(ref _netServerSCUdp, _serverConfig.udpPort);
        }

        void OnPlayerDisconnected(IPeer peer){
            if (_peerId2Player.TryGetValue(peer.Id, out var player)) {
                Log("OnPlayerDisconnected  " + player.Account);
                _peerId2Player.Remove(peer.Id);
                player.Room.OnPlayerDisconnect(player);
            }
        }

        protected override void OnMasterServerInfo(ServerIpInfo info){
            if (info.ServerType == (byte) EServerType.DatabaseServer) {
                ReqOtherServerInfo(EServerType.DatabaseServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientDS(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
            }

            if (info.ServerType == (byte) EServerType.LobbyServer) {
                ReqOtherServerInfo(EServerType.LobbyServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientLG(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
            }
        }

        private void InitClientLG(ServerIpInfo info){
            InitNetClient(ref _netClientLG, info.Ip, info.Port, OnLobbyConn);
        }

        private void InitClientDS(ServerIpInfo info){
            InitNetClient(ref _netClientDS, info.Ip, info.Port, OnDBConn);
        }

        private void OnLobbyConn(){
            _netClientLG.SendMessage(EMsgLS.G2L_RegisterServer, new Msg_RegisterServer {
                ServerInfo = new ServerIpInfo() {
                    ServerType = (byte) serverType,
                    Ip = this.Ip,
                    Port = _serverConfig.tcpPort
                }
            });
            Debug.Log("OnLobbyConn");
        }

        private void OnDBConn(){
            Debug.Log("OnDBConn");
        }

        protected void L2G_UserLeave(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_UserLeave>();
            Log("L2G_UserLeave " + msg);
            if (_roomId2Rooms.TryGetValue(msg.RoomId, out var room)) {
                room.OnPlayerLeave(msg.UserId);
            }
        }

        protected void L2G_UserReconnect(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_UserReconnect>();
            Log("L2G_UserReconnect " + msg);
            if (_uid2Players.TryGetValue(msg.PlayerInfo.UserId, out var player)) {
                var room = player.Room;
                if (room != null) {
                    room.OnPlayerReconnect(msg.PlayerInfo);
                    reader.Respond(1, EResponseStatus.Success);
                    return;
                }
            }

            reader.Respond(0, EResponseStatus.Failed);
        }

        protected void L2G_CreateRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_CreateRoom>();
            var room = CreateGame(msg.GameType) as BaseRoom;
            if (room == null) {
                reader.Respond(-1, EResponseStatus.Failed);
                return;
            }

            room.TcpEnd = TcpEnd;
            room.UdpEnd = UdpEnd;
            Debug.Log("L2G_CreateRoom" + msg);
            var roomId = CurRoomId++;
            _roomId2Rooms.Add(roomId, room);
            _cachedBaseRooms = null;

            room.DoStart(this, roomId, msg.GameType, msg.MapId, msg.Players, msg.GameHash);
            var players = room.Players;
            foreach (var player in players) {
                _uid2Players[player.UserId] = player;
                //get game data from database
                _netClientDS.SendMessage(EMsgDS.S2D_ReqGameData, new Msg_S2D_ReqGameData() {
                        account = player.Account
                    }, (status, respond) => {
                        var rMsg = respond.Parse<Msg_D2S_RepGameData>();
                        Debug.Log("Msg_D2S_RepGameData" + rMsg);
                        if (_uid2Players.TryGetValue(player.UserId, out var user)) {
                            user.GameData = rMsg.data;
                            if (user.Room != null && user.Room.RoomId == roomId) { //防止玩家重新进入房间
                                user.Room.OnRecvPlayerGameData(user);
                            }
                        }
                    }
                );
            }


            //Load dlls
            reader.Respond(roomId, EResponseStatus.Success);
        }

        private Dictionary<int, Player> _peerId2Player = new Dictionary<int, Player>();

        protected void C2G_Hello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_Hello>().Hello;
            Debug.Log("C2G_Hello" + msg + " id" + reader.Peer.Id);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var room = _roomId2Rooms.GetRefVal(msg.RoomId);
            if (room != null
                && room.GameType == msg.GameType
                && room.GameHash == msg.GameHash
            ) {
                var localId = room.GetUserLocalId(userInfo.UserId);
                if (localId != -1) {
                    var player = room.Players[localId];
                    if (player.LoginHash == userInfo.LoginHash) {
                        player.PeerTcp = reader.Peer;
                        _peerId2Player[reader.Peer.Id] = player;
                        reader.Peer.AddExtension(player);
                        reader.Respond(EMsgSC.G2C_Hello, new Msg_G2C_Hello() {
                            LocalId = (byte) localId,
                            UserCount = (byte) room.MaxPlayerCount,
                            MapId = room.MapId,
                            RoomId = room.RoomId,
                            Seed = room.Seed,
                            UdpEnd = new IPEndInfo() {
                                Ip = this.Ip,
                                Port = _serverConfig.udpPort
                            }
                        });
                        room.OnPlayerConnect(player);
                        return;
                    }
                }
            }

            reader.Respond(1, EResponseStatus.Failed);
        }


        protected void C2G_UdpHello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_UdpHello>().Hello;
            Debug.Log("C2G_UdpHello" + msg + " id" + reader.Peer.Id);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var room = _roomId2Rooms.GetRefVal(msg.RoomId);
            if (room != null
                && room.GameType == msg.GameType
                && room.GameHash == msg.GameHash
            ) {
                var localId = room.GetUserLocalId(userInfo.UserId);
                if (localId != -1) {
                    var player = room.Players[localId];
                    if (player.LoginHash == userInfo.LoginHash) {
                        player.PeerUdp = reader.Peer;
                        reader.Peer.AddExtension(player);
                        reader.Respond(EMsgSC.G2C_UdpHello, new Msg_G2C_Hello() {
                            MapId = room.MapId,
                            LocalId = (byte) localId
                        });
                    }
                }
            }
        }

        protected void C2G_UdpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                reader.Peer?.Disconnect("");
                Debug.Log("C2G_UdpMessage: Error msg unknown user peerId = " + reader.Peer.Id);
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Room.OnRecvMsg(player, deserializer);
        }

        protected void C2G_TcpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                reader.Peer.Disconnect("");
                Debug.Log("C2G_TcpMessage: Error msg unknown user peerId = " + reader.Peer.Id);
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Room.OnRecvMsg(player, deserializer);
        }

        private delegate IRoom FuncCreateRoom();

        private Dictionary<int, FuncCreateRoom> _roomFactoryFuncs = new Dictionary<int, FuncCreateRoom>();

        string RoomType2DllPath(int type){
            return "Game.Tank" + ".dll"; //TODO ReadFromConfig
        }

        /// Create From Dll by reflect 
        private IRoom CreateGame(int type){
            //TODO Pool
            if (_roomFactoryFuncs.TryGetValue(type, out FuncCreateRoom _func)) {
                return _func?.Invoke();
            }

            var path = RoomType2DllPath(type);
            if (path == null) {
                return null;
            }

            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            Debug.Log("Load dll " + dllPath);
            if (!File.Exists(dllPath)) {
                Debug.LogError("Load dll failed  " + dllPath);
                _roomFactoryFuncs[type] = null;
                return null;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            var types = assembly.GetTypes().Where(
                t => t.IsSubclassOf(typeof(BaseRoom))
            ).ToArray();
            if (types.Length != 1) {
                Debug.LogError("dll do not have type of IRoom :" + dllPath);
                _roomFactoryFuncs[type] = null;
                return null;
            }

            FuncCreateRoom factory = () => { return (IRoom) System.Activator.CreateInstance(types[0], true); };
            _roomFactoryFuncs[type] = factory;
            var room = factory();
            if (room == null) {
                Debug.LogError($"Can not load Game Dll type = {type}");
                return null;
            }

            return room;
        }
    }
}