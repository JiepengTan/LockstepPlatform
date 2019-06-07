using System.Collections.Generic;
using System.Net.NetworkInformation;
using Lockstep.Logging;
using NetMsg.Server;
using Lockstep.Server.Common;
using Lockstep.Networking;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public class GameServer : Common.Server {
        private NetServer<EMsgSC> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;
        protected NetClient<EMsgLS> _netClientLG; //其他类型的Master 用于提供服务

        private ServerIpInfo lobbyInfo;

        public override void DoStart(){
            base.DoStart();
            InitServerSC();
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort);
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
            lobbyInfo = info;
            InitNetClient(ref _netClientLG, info.Ip, info.Port, OnLobbyConn);
        }

        private void InitClientDS(ServerIpInfo info){
            InitNetClient(ref _netClientDS, info.Ip, info.Port, OnDBConn);
        }

        private void OnLobbyConn(){
            _netClientLG.SendMessage(EMsgLS.G2L_RegisterServer,new Msg_RegisterServer {
                ServerInfo = new ServerIpInfo() {
                    ServerType = (byte) serverType,
                    Ip = this.IP,
                    Port = _serverConfig.tcpPort
                }
            });
            Debug.Log("OnLobbyConn");
        }

        private void OnDBConn(){
            Debug.Log("OnDBConn");
        }

        public class Player : BaseRecyclable {
            public long UserId;
            public string Account;
            public string LoginHash;
            public byte LocalId;
            public IPeer Peer;

            public override void OnRecycle(){
                Peer = null;
            }
        }

        public class GameRoom : BaseRecyclable {
            public int MapId;
            public int RoomId;
            public string GameHash;
            public int GameType;
            public Player[] Players;
            public Dictionary<long, byte> userId2LocalId = new Dictionary<long, byte>();

            public void Init(){
                userId2LocalId.Clear();
                var userCount = Players.Length;
                for (byte i = 0; i < userCount; i++) {
                    var player = Players[i];
                    if (player != null) {
                        userId2LocalId.Add(player.UserId, player.LocalId);
                    }
                }
            }

            public override void OnRecycle(){
                if (Players == null) return;
                foreach (var player in Players) {
                    Pool.Return(player);
                }

                Players = null;
            }

            public int GetUserLocalId(long userId){
                if (userId2LocalId.TryGetValue(userId, out var id)) {
                    return id;
                }

                return -1;
            }

            public int CheckAndGetUserLocalId(GamePlayerInfo user){
                if (userId2LocalId.TryGetValue(user.UserId, out var id)) {
                    if (Players[id].LoginHash == user.LoginHash)
                        return id;
                }

                return -1;
            }
        }

        private Dictionary<int, GameRoom> _roomId2Rooms = new Dictionary<int, GameRoom>();
        private int CurRoomId;

        protected void L2G_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_StartGame>();
            Debug.Log("L2G_StartGame" + msg);
            var room = Pool.Get<GameRoom>();
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
            }

            room.Players = players;
            room.Init();
            _roomId2Rooms.Add(roomId, room);
            reader.Respond(roomId, EResponseStatus.Success);
        }

        protected void C2G_Hello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_Hello>();
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
                    player.Peer = reader.Peer;
                    reader.Peer.AddExtension(player);
                    reader.Respond(EMsgSC.G2C_Hello, new Msg_G2C_Hello() {
                        MapId = room.MapId,
                        LocalId = (byte) localId
                    });
                    return;
                }
            }

            reader.Respond(1, EResponseStatus.Failed);
        }


        #region old

#if false
    public class GameServer : BaseServer, IGameServer {
        //account map
        private Dictionary<string, long> account2PlayerId = new Dictionary<string, long>();
        private long PlayerAutoIncId = 1;
        private Dictionary<long, IRoom> playerId2Room = new Dictionary<long, IRoom>();

        //Player map
        private Dictionary<long, Player> playerID2Player = new Dictionary<long, Player>();
        private Dictionary<int, Player> netId2Player = new Dictionary<int, Player>();
        private Dictionary<int, Player> netId2PlayerRoom = new Dictionary<int, Player>();

        //room map
        private int RoomAutoIncId = 1;
        private List<IRoom> _allRooms = new List<IRoom>();
        private Dictionary<int, IRoom> roomId2Room = new Dictionary<int, IRoom>();
        private Dictionary<int, List<IRoom>> gameId2Rooms = new Dictionary<int, List<IRoom>>();


        private const int MAX_NAME_LEN = 30;

        public NetServer<EMsgSC, INetProxy> serverLobby;
        public NetServer<EMsgSC, INetProxy> serverRoom;

        public const byte MAX_HANDLER_IDX = (byte) EMsgSC.EnumCount;
        public const byte INIT_MSG_IDX = (byte) EMsgSC.C2L_ReqLogin;
        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgSC.EnumCount];

        private delegate IRoom FuncCreateRoom();

        private delegate void DealNetMsg(Player player, Deserializer reader);

        private Dictionary<int, FuncCreateRoom> _roomFactoryFuncs = new Dictionary<int, FuncCreateRoom>();

        private string RoomIP = "";
        private ushort RoomPort = 0;

        //TODO read from config
        string RoomType2DllPath(int type){
            return "Game.Tank" + ".dll";
        }


        #region LifeCycle

        public override void DoStart(){
            base.DoStart();
            RegisterMsgHandlers();
            //serverLobby = new NetServer(Define.ClientKey);
            //serverLobby.DataReceived += OnDataReceived;
            //serverLobby.ClientConnected += OnClientConnected;
            //serverLobby.ClientDisconnected += OnCilentDisconnected;
            //serverLobby.Run(tcpPort);
            //this.RoomPort = udpPort;
            //this.RoomIP = "127.0.0.1";
            //serverRoom = new NetServer(Define.ClientKey);
            //serverRoom.DataReceived += OnDataReceivedRoom;
            //serverRoom.ClientConnected += OnClientConnectedRoom;
            //serverRoom.ClientDisconnected += OnCilentDisconnectedRoom;
            //serverRoom.Run(udpPort);
            //Debug.Log($"Listen tcpPort {tcpPort} udpPort {udpPort}");
        }

        public void OnDataReceivedRoom(NetPeer peer, byte[] data){
            int netID = peer.Id;
            try {
                var reader = new Deserializer(Compressor.Decompress(data));
                var playerID = reader.GetInt64();
                var player = GetPlayer(playerID);
                if (player.gameSock == null) {
                    player.gameSock = peer;
                    netId2PlayerRoom.Add(peer.Id, player);
                }

                var room = player.room;
                if (room == null) {
                    Debug.LogError($"MsgError:Player {player.PlayerId} not in room");
                    return;
                }

                room.OnRecvMsg(player, reader);
            }
            catch (Exception e) {
                Debug.LogError($"netID{netID} parse msg Error:{e.ToString()}");
            }
        }

        public void OnClientConnectedRoom(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnClientConnectedRoom netID = {peer.Id}");
        }

        public void OnCilentDisconnectedRoom(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnCilentDisconnectedRoom netID = {peer.Id}");
            var player = GetPlayerRoom(peer.Id);
            if (player != null) {
                RemovePlayer(player);
            }
        }


        public override void DoUpdate(int deltaTime){
            foreach (var room in _allRooms) {
                try {
                    room?.DoUpdate(deltaTime);
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public override void DoDestroy(){ }

        public override void PollEvents(){
            serverLobby?.PollEvents();
            serverRoom?.PollEvents();
        }

        #endregion

        #region rooms

        public List<IRoom> GetRooms(int roomType){
            return DictExtensions.GetRefVal(gameId2Rooms, roomType);
        }

        public IRoom GetRoom(int roomId){
            return DictExtensions.GetRefVal(roomId2Room, roomId);
        }

        public IRoom GetRoomByUserID(int id){
            var player = GetPlayerLobby(id);
            if (player != null) {
                return player.room;
            }

            return null;
        }

        public void RemoveRoom(IRoom room){
            var ids = room.GetAllPlayerIDs();
            if (ids != null) {
                foreach (var playerId in ids) {
                    if (playerId2Room.TryGetValue(playerId, out var tRoom)) {
                        if (tRoom.RoomId == room.RoomId) {
                            playerId2Room.Remove(playerId);
                        }
                    }
                }
            }

            roomId2Room.Remove(room.RoomId);
            _allRooms.Remove(room);
            gameId2Rooms[room.TypeId].Remove(room);
            if (gameId2Rooms[room.TypeId].Count == 0) {
                gameId2Rooms.Remove(room.TypeId);
            }

            room.DoDestroy();
        }

        /// Create From Dll by reflect 
        private IRoom CreateRoom(int type){
            //TODO Pool
            if (_roomFactoryFuncs.TryGetValue(type, out FuncCreateRoom _func)) {
                return _func?.Invoke();
            }

            var path = RoomType2DllPath(type);
            if (path == null) {
                return null;
            }

            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            var assembly = Assembly.LoadFrom(dllPath);
            Debug.Log("Load dll " + dllPath);
            if (assembly == null) {
                Debug.LogError("Load dll failed  " + dllPath);
                _roomFactoryFuncs[type] = null;
                return null;
            }

            var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IRoom))).ToArray();
            if (types.Length != 1) {
                Debug.LogError("dll do not have type of IRoom :" + dllPath);
                _roomFactoryFuncs[type] = null;
                return null;
            }

            FuncCreateRoom factory = () => { return (IRoom) System.Activator.CreateInstance(types[0], true); };
            _roomFactoryFuncs[type] = factory;
            return factory();
        }


        public IRoom CreateRoom(int type, Player master, string roomName, byte size){
            if (RoomAutoIncId == int.MaxValue - 1) {
                RoomAutoIncId = 0;
            }

            var id = ++RoomAutoIncId;

            IRoom room = CreateRoom(type);
            if (room == null) {
                Debug.LogError($"Can not load game DLL type = {type} roomName = {roomName}");
                return null;
            }

            Debug.Log($"CreateRoom type = {type} name = {roomName}");
            roomId2Room.Add(id, room);
            _allRooms.Add(room);
            if (gameId2Rooms.TryGetValue(type, out var roomLst)) {
                roomLst.Add(room);
            }
            else {
                var lst = new List<IRoom>();
                lst.Add(room);
                gameId2Rooms.Add(type, lst);
            }

            room.DoStart(type, id, this, size, roomName);
            room.OnPlayerEnter(master);
            SendCreateRoomResult(master);
            return room;
        }


        private void SendCreateRoomResult(Player player){
            var writer = new Serializer();
            writer.PutByte((byte) EMsgSC.L2C_CreateRoom);
            new Msg_CreateRoomResult() {roomId = player.RoomId}.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            player.SendLobby(bytes);
        }

        public bool JoinRoom(Player player, int roomId){
            var room = GetRoom(roomId);
            if (room == null) {
                Debug.Log($"player{player.PlayerId} try to enter a room which not exist {roomId} ");
                return false;
            }

            if (player.status != EPlayerStatus.Idle) {
                Debug.Log($"player status {player.status} can not sit down");
                return false;
            }

            if (player.room != null) {
                Debug.Log($"player  {player.PlayerId} already in room, should leave the room first");
                return false;
            }

            playerId2Room[player.PlayerId] = room;
            room.OnPlayerEnter(player);
            return true;
        }

        public bool LeaveRoom(Player player){
            return true;
        }

        #endregion

        #region player

        public void TickOut(Player player, int reason){
            Debug.LogError($"TickPlayer reason:{reason} {player.ToString()}");
            player.lobbySock.Disconnect();
        }

        public Player GetPlayer(long playerId){
            return DictExtensions.GetRefVal(playerID2Player, playerId);
        }

        public Player GetPlayerLobby(int netId){
            return DictExtensions.GetRefVal(netId2Player, netId);
        }

        public Player GetPlayerRoom(int netId){
            return DictExtensions.GetRefVal(netId2PlayerRoom, netId);
        }


        public void RemovePlayer(Player player){
            if (player.lobbySock == null) return;
            playerID2Player.Remove(player.PlayerId);
            netId2Player.Remove(player.lobbySock.Id);
            if (player.gameSock != null) netId2PlayerRoom.Remove(player.gameSock.Id);
            player.gameSock = null;
            player.lobbySock = null;
            player.room?.OnDisconnect(player);
            player.room = null;
            player.status = EPlayerStatus.Idle;
        }

        public Player AddPlayer(NetPeer peer){
            if (PlayerAutoIncId >= long.MaxValue - 1) {
                PlayerAutoIncId = 1;
            }

            var playerID = PlayerAutoIncId++;
            return CreatePlayer(playerID, peer);
        }

        public Player CreatePlayer(long playerID, NetPeer peer){
            var netID = peer.Id;
            var player = new Player();
            player.PlayerId = playerID;
            player.lobbySock = peer;
            netId2Player[netID] = player;
            playerID2Player[playerID] = player;
            return player;
        }

        #endregion

        #region Conn status

        //Net infos
        public void OnClientConnected(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnClientConnected netID = {peer.Id}");
        }

        public void OnCilentDisconnected(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnCilentDisconnected netID = {peer.Id}");
            var player = GetPlayerLobby(peer.Id);
            if (player != null) {
                RemovePlayer(player);
            }
        }

        #endregion

        #region Msg Handler

        public void OnDataReceived(NetPeer peer, byte[] data){
            int netID = peer.Id;
            try {
                var reader = new Deserializer(Compressor.Decompress(data));
                var msgType = reader.GetByte();
                if (msgType >= MAX_HANDLER_IDX) {
                    Debug.LogError("msgType out of range " + msgType);
                    return;
                }

                if (msgType == (byte) EMsgSC.C2L_ReqLogin) {
                    Msg_ReqLogin initMsg = null;
                    try {
                        initMsg = reader.Parse<Msg_ReqLogin>();
                    }
#pragma warning disable 168
                    catch (Exception _e) {
#pragma warning restore 168
                        return;
                    }

                    var ep = peer.EndPoint;
                    DealLogin(initMsg, peer, reader);
                    return;
                }

                //Debug.Log($"OnDataReceived netID = {netID}  type:{(EMsgCL)msgType}");
                {
                    if (!CheckMsg(reader, netID, out var player)) return;
                    var _func = allMsgDealFuncs[msgType];
                    if (_func != null) {
                        _func(player, reader);
                    }
                    else {
                        Debug.LogError("ErrorMsg type :no msgHnadler" + msgType);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError($"netID{netID} parse msg Error:{e.ToString()}");
            }
        }


        private void DealLogin(Msg_ReqLogin initMsg, NetPeer peer, Deserializer reader){
            var netId = peer.Id;
            var account = initMsg.account;
            Player player = null;
            if (account2PlayerId.TryGetValue(account, out long id)) {
                player = GetPlayer(id);
                if (player == null) {
                    player = CreatePlayer(id, peer);
                }
            }
            else {
                player = AddPlayer(peer);
                account2PlayerId.Add(account, player.PlayerId);
            }

            IRoom room = null;
            if (playerId2Room.TryGetValue(player.PlayerId, out IRoom tRoom)) {
                room = tRoom;
                player.room = room;
                room.OnReconnect(player);
            }

            var writer = new Serializer();
            writer.PutByte((byte) EMsgSC.L2C_RepLogin);
            var msg = new Msg_RepLogin() {playerId = player.PlayerId};
            if (room != null) {
                msg.roomId = room.RoomId;
                msg.port = RoomPort;
                msg.ip = RoomIP;
                msg.childMsg = room.GetReconnectMsg(player);
            }
            else {
                msg.roomId = -1;
                msg.ip = "";
            }

            Debug.Log($"Deal Init msg roomId {msg.roomId} isReconnect {room != null}");
            msg.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            player.SendLobby(bytes);
        }


        private void RegisterMsgHandlers(){
            //RegisterNetMsgHandler(EMsgSC.C2L_JoinRoom, JoinRoom);
            //RegisterNetMsgHandler(EMsgSC.C2L_CreateRoom, CreateRoom);
            //RegisterNetMsgHandler(EMsgSC.C2L_LeaveRoom, LeaveRoom);
            //RegisterNetMsgHandler(EMsgSC.C2L_RoomMsg, RoomMsg);
        }

        private void RegisterNetMsgHandler(EMsgSC type, DealNetMsg func){
            allMsgDealFuncs[(int) type] = func;
        }

        private bool CheckMsg(Deserializer reader, int netID, out Player player){
            player = GetPlayerLobby(netID);
            if (player == null) {
                Debug.LogError($"ErrorMsg: have no player {netID}");
            }

            return player != null;
        }

        private void CreateRoom(Player player, Deserializer reader){
            var msg = reader.Parse<Msg_CreateRoom>();
            if (_allRooms.Count > 0) {
                JoinRoom(player, _allRooms[0].RoomId);
                SendCreateRoomResult(player);
            }
            else {
                CreateRoom(msg.type, player, msg.name, msg.size);
            }
        }

        private void LeaveRoom(Player player, Deserializer reader){
            var room = player.room;
            if (room == null) {
                Debug.LogError($"MsgError:Player {player.PlayerId} not in room");
            }

            room.OnPlayerLeave(player);
        }

        private void JoinRoom(Player player, Deserializer reader){
            var msg = reader.Parse<Msg_JoinRoom>();
            JoinRoom(player, msg.roomId);
        }

        private void RoomMsg(Player player, Deserializer reader){
            var room = player.room;
            if (room == null) {
                Debug.LogError($"MsgError:Player {player.PlayerId} not in room");
                return;
            }

            room.OnRecvMsg(player, reader);
        }

        #endregion
    }
}
#endif

        #endregion
    }
}