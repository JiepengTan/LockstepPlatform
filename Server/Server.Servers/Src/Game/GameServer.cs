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
    public class GameServer : Common.Server {
        private NetServer<EMsgSC> _netServerSCUdp;
        private NetServer<EMsgSC> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;
        protected NetClient<EMsgLS> _netClientLG; //其他类型的Master 用于提供服务

        private Dictionary<int, Room> _roomId2Rooms = new Dictionary<int, Room>();
        private Dictionary<long, Player> _uid2Players = new Dictionary<long, Player>();

        private int CurRoomId;


        public override void DoStart(){
            base.DoStart();
            InitServerSC();
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort);
            InitNetServer(ref _netServerSCUdp, _serverConfig.udpPort);
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


        protected void L2G_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_StartGame>();
            var game = CreateGame(msg.GameType);
            if (game == null) {
                reader.Respond(-1, EResponseStatus.Failed);
            }

            Debug.Log("L2G_StartGame" + msg);
            var room = Pool.Get<Room>();
            var roomId = CurRoomId++;
            room.GameType = msg.GameType;
            room.GameHash = msg.GameHash;
            room.MapId = msg.MapId;
            var count = msg.Players.Length;
            var players = new Player[count];
            for (int i = 0; i < count; i++) {
                var user = msg.Players[i];
                var player = Pool.Get<Player>();
                player.UserId = user.UserId;
                player.Account = user.Account;
                player.LoginHash = user.LoginHash;
                player.LocalId = (byte) i;
                players[i] = player;
                _uid2Players[user.UserId] = player;
            }

            room.Players = players;
            room.Init(game);
            _roomId2Rooms.Add(roomId, room);
            //Load dlls
            reader.Respond(roomId, EResponseStatus.Success);
        }

        protected void C2G_Hello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_Hello>().Hello;
            Debug.Log("C2G_Hello" + msg);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var room = _roomId2Rooms.GetRefVal(msg.RoomId);
            if (room != null
                && room.GameType == msg.GameType
                && room.GameHash == msg.GameHash
            ) {
                var localId = room.CheckAndGetUserLocalId(userInfo);
                var userId = userInfo.UserId;
                var retTimeStamp = Time.timeSinceLevelLoad;
                var roomId = room.RoomId;
                if (localId != -1) {
                    var player = room.Players[localId];
                    player.PeerTcp = reader.Peer;
                    player.SetRoom(room);
                    reader.Peer.AddExtension(player);
                    reader.Respond(EMsgSC.G2C_Hello, new Msg_G2C_Hello() {
                        MapId = room.MapId,
                        LocalId = (byte) localId,
                        UdpEnd = new IPEndInfo() {
                            Ip =  this.Ip,
                            Port = _serverConfig.udpPort
                        }
                        
                    });
                    //get game data from database
                    _netClientDS.SendMessage(EMsgDS.S2D_ReqGameData, new Msg_S2D_ReqGameData() {
                            account = userInfo.Account
                        }, (status, respond) => {
                            var rMsg = respond.Parse<Msg_D2S_RepGameData>();
                            if (_uid2Players.TryGetValue(userId, out var user)) {
                                user.GameData = rMsg.data;
                                if (user.Room != null && user.Room.RoomId == roomId) { //防止玩家重新进入房间
                                    if (user.Room.TimeSinceCreate <= retTimeStamp) {
                                        user.Room.OnRecvPlayerGameData(user);
                                    }
                                }
                            }
                        }
                    );
                    return;
                }
            }

            reader.Respond(1, EResponseStatus.Failed);
        }


        protected void C2G_UdpHello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_UdpHello>().Hello;
            Debug.Log("C2G_Hello" + msg);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var room = _roomId2Rooms.GetRefVal(msg.RoomId);
            if (room != null
                && room.GameType == msg.GameType
                && room.GameHash == msg.GameHash
            ) {
                var localId = room.CheckAndGetUserLocalId(userInfo);
                if (localId != -1) {
                    var player = room.Players[localId];
                    player.PeerUdp = reader.Peer;
                    reader.Peer.AddExtension(player);
                    reader.Respond(EMsgSC.G2C_UdpHello, new Msg_G2C_Hello() {
                        MapId = room.MapId,
                        LocalId = (byte) localId
                    });
                }
            }
        }

        protected void C2G_UdpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                Debug.Log("Error msg unknow user");
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Room.OnRecvMsg(player, deserializer);
        }
        protected void C2G_TcpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                Debug.Log("Error msg unknow user");
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Room.OnRecvMsg(player, deserializer);
        }
        private delegate IGame FuncCreateRoom();

        private Dictionary<int, FuncCreateRoom> _roomFactoryFuncs = new Dictionary<int, FuncCreateRoom>();

        string RoomType2DllPath(int type){
            return "Game.Tank" + ".dll";//TODO ReadFromConfig
        }

        /// Create From Dll by reflect 
        private IGame CreateGame(int type){
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
                t => t.IsSubclassOf(typeof(BaseGame))
            ).ToArray();
            if (types.Length != 1) {
                Debug.LogError("dll do not have type of IRoom :" + dllPath);
                _roomFactoryFuncs[type] = null;
                return null;
            }

            FuncCreateRoom factory = () => { return (IGame) System.Activator.CreateInstance(types[0], true); };
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