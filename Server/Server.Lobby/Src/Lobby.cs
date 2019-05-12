using System;
using System.Collections.Generic;
using LiteNetLib;
using Lockstep.Serialization;
using Lockstep.Logging;
using NetMsg.Lobby;
using Server.Common;

namespace Lockstep.Logic.Server {
    public class Lobby : ILobby {
        //TCP
        private Dictionary<long, Player> playerID2Player = new Dictionary<long, Player>();
        private Dictionary<int, NetPeer> netId2NetPeer = new Dictionary<int, NetPeer>();
        private Dictionary<int, Player> netID2Player = new Dictionary<int, Player>();

        private Dictionary<int, IRoom> roomId2Room = new Dictionary<int, IRoom>();
        private List<IRoom> _allRooms = new List<IRoom>();
        private Dictionary<int, List<IRoom>> gameId2Rooms = new Dictionary<int, List<IRoom>>();

        private static long PlayerAutoIncID = 0;
        private static int RoomAutoIncID = 0;
        private const int MAX_NAME_LEN = 30;

        public NetServer server;

        public const byte MAX_HANDLER_IDX = (byte) EMsgCL.EnumCount;
        public const byte INIT_MSG_IDX = (byte) EMsgCL.C2L_InitMsg;
        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgCL.EnumCount];

        private delegate void DealNetMsg(Player player, Deserializer reader);

        #region LifeCycle

        public void DoStart(int port){
            RegisterMsgHandlers();
            server = new NetServer(Define.ClientKey);
            server.DataReceived += OnDataReceived;
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnCilentDisconnected;
            server.Run(port);
        }

        public void DoUpdate(int deltaTime){
            foreach (var room in _allRooms) {
                try {
                    room?.DoUpdate(deltaTime);
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public void DoDestroy(){ }

        public void PollEvents(){
            server?.PollEvents();
        }

        #endregion

        #region rooms

        public List<IRoom> GetRooms(int roomType){
            return gameId2Rooms.GetRefVal(roomType);
        }

        public IRoom GetRoom(int roomId){
            return roomId2Room.GetRefVal(roomId);
        }

        public IRoom GetRoomByUserID(int id){
            var player = GetPlayer(id);
            if (player != null) {
                return player.room;
            }

            return null;
        }

        public void RemoveRoom(IRoom room){
            roomId2Room.Remove(room.RoomId);
            _allRooms.Remove(room);
            gameId2Rooms[room.TypeId].Remove(room);
            if (gameId2Rooms[room.TypeId].Count == 0) {
                gameId2Rooms.Remove(room.TypeId);
            }

            room.DoDestroy();
        }

        public IRoom CreateRoom(int type, Player master, string roomName){
            if (RoomAutoIncID == int.MaxValue - 1) {
                RoomAutoIncID = 0;
            }

            var id = ++RoomAutoIncID;
            IRoom room = null;//new Room(); //TODO should load dll 
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

            room.DoStart(type, id, this, 1, roomName);
            room.OnPlayerEnter(master);
            return room;
        }

        public bool JoinRoom(long playerID, int roomID){
            var player = GetPlayer(playerID);
            if (player == null) {
                Debug.LogError($"null player  {playerID} join room {roomID} ");
                return false;
            }

            var room = GetRoom(roomID);
            if (room == null) {
                Debug.LogError($"player{playerID} try to enter a room which not exist {roomID} ");
                return false;
            }

            if (player.status != EPlayerStatus.Idle) {
                Debug.LogError($"player status {player.status} can not sit down");
                return false;
            }

            if (player.room != null) {
                Debug.LogError($"player  {playerID} already in room, should leave the room first");
                return false;
            }

            room.OnPlayerEnter(player);
            player.room = room;
            player.status = EPlayerStatus.Sit;
            return true;
        }

        public bool LeaveRoom(long playerID){
            var player = GetPlayer(playerID);
            if (player == null) {
                Debug.LogError($"null player  {playerID} leave room ");
                return false;
            }

            var room = player.room;
            if (room == null) {
                Debug.LogError($"player {playerID} not in room, can not leave");
                return false;
            }

            player.room.OnPlayerLeave(player);
            player.room = null;
            player.status = EPlayerStatus.Idle;
            return true;
        }

        #endregion

        #region player
        public void TickOut(Player player,int reason){
            Debug.LogError($"TickPlayer reason:{reason} {player.ToString()}");
            player.socket.Disconnect();
        }
        public Player GetPlayer(long playerId){
            return playerID2Player.GetRefVal(playerId);
        }

        public Player GetPlayer(int netID){
            return netID2Player.GetRefVal(netID);
        }

        public Player AddPlayer(int netID){
            if (PlayerAutoIncID >= long.MaxValue - 1) {
                PlayerAutoIncID = 0;
            }

            var playerID = ++PlayerAutoIncID;
            return CreatePlayer(playerID, netID);
        }

        public void RemovePlayer(int netID){
            netId2NetPeer[netID] = null;
            if (netID2Player.TryGetValue(netID, out var player)) {
                netID2Player[netID] = null;
                playerID2Player[player.PlayerId] = null;
            }
        }

        public Player CreatePlayer(long playerID, int netID){
            var player = new Player();
            player.PlayerId = playerID;
            player.netID = netID;
            player.socket = netId2NetPeer[netID];
            netID2Player[netID] = player;
            playerID2Player[playerID] = player;
            return player;
        }

        #endregion

        #region Conn status

        //Net infos
        public void OnClientConnected(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnClientConnected netID = {peer.Id}");
            netId2NetPeer[peer.Id] = peer;
        }

        public void OnCilentDisconnected(object objPeer){
            var peer = (NetPeer) objPeer;
            Debug.Log($"OnCilentDisconnected netID = {peer.Id}");
            var player = GetPlayer(peer.Id);
            LeaveRoom(player.PlayerId);
            RemovePlayer(peer.Id);
        }

        #endregion

        #region Msg Handler
        
 
        public void OnDataReceived(int netID, byte[] data){
            try {
                realOnDataReceived(netID, data);
            }
            catch (Exception e) {
                Debug.LogError($"netID{netID} parse msg Error:{e.ToString()}");
            }
        }

        public void realOnDataReceived(int netID, byte[] data){
            var reader = new Deserializer(Compressor.Decompress(data));
            var msgType = reader.GetByte();
            var playerID = reader.GetLong();
            if (msgType >= MAX_HANDLER_IDX) {
                Debug.LogError("msgType outof range");
                return;
            }

            //Debug.Log($"OnDataReceived netID = {netID}  type:{(EMsgCL)msgType}");
            {
                if (CheckMsg(reader, netID, playerID, msgType == (byte) EMsgCL.C2L_InitMsg, out var player)) return;
                var _func = allMsgDealFuncs[msgType];
                if (_func != null) {
                    _func(player, reader);
                }
                else {
                    Debug.LogError("ErrorMsg type :no msgHnadler" + msgType);
                }
            }
        }

        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgCL.C2L_InitMsg, OnMsg_InitMsg);
            RegisterNetMsgHandler(EMsgCL.C2L_JoinRoom, OnMsg_JoinRoom);
            RegisterNetMsgHandler(EMsgCL.C2L_CreateRoom, OnMsg_CreateRoom);
            RegisterNetMsgHandler(EMsgCL.C2L_LeaveRoom, OnMsg_LeaveRoom);
            RegisterNetMsgHandler(EMsgCL.C2L_RoomMsg, OnMsg_RoomMsg);
        }

        private void RegisterNetMsgHandler(EMsgCL type, DealNetMsg func){
            allMsgDealFuncs[(int) type] = func;
        }

        private bool CheckMsg(Deserializer reader, int netID, long playerID, bool isInit, out Player player){
            if (isInit) { //初始化信息处理
                var isReconn = playerID > 0;
                player = GetPlayer(playerID);
                if (isReconn) {
                    if (player == null) {
                        player = CreatePlayer(playerID, netID);
                    }
                }
                else {
                    player = AddPlayer(netID);
                }
            }
            else {
                player = GetPlayer(playerID);
            }

            if (player == null) {
                Debug.LogError($"ErrorMsg: have no player {playerID}");
                return true;
            }

            if (player.netID != netID) {
                Debug.LogError($"ErrorMsg: netID error: faker? netID{netID}!= player.netID = {player.netID}");
                return true;
            }

            return false;
        }

        private void OnMsg_InitMsg(Player player, Deserializer reader){
            var initMsg = reader.Parse<InitMsg>();
            player.name = initMsg.name;
            SendReqInit(player);
        }

        private void SendReqInit(Player player){
            player.Send((byte)EMsgCL.L2C_ReqInit, new ReqInit() {playerId = player.PlayerId});
        }


        private void OnMsg_CreateRoom(Player player, Deserializer reader){
            var msg = reader.Parse<CreateRoom>();
            CreateRoom(msg.type, player, msg.name);
        }

        private void OnMsg_LeaveRoom(Player player, Deserializer reader){ }
        private void OnMsg_JoinRoom(Player player, Deserializer reader){ }

        private void OnMsg_RoomMsg(Player player, Deserializer reader){
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