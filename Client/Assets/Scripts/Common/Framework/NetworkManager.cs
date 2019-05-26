using System.Collections.Generic;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Game;
using NetMsg.Lobby;
using UnityEngine;

namespace Lockstep.Game {
    public partial class NetworkManager : SingletonManager<NetworkManager>, INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 9050;

        private BaseNetProxy _netProxyLobby;
        private BaseNetProxy _netProxyRoom;

        private long _playerID;
        private int _roomId;

        private bool isReconnected = false; //是否是重连
        public int Ping => _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected => _netProxyLobby != null && _netProxyLobby.Connected;

        public override void DoAwake(IServiceContainer services){
            _netProxyRoom = new BaseNetProxy((int) EMsgCS.EnumCount);
            _netProxyLobby = new BaseNetProxy((int) EMsgCL.EnumCount);
            _eventRegisterService.RegisterEvent<EMsgCL, NetMsgHandler>("OnMsg_L2C", "OnMsg_".Length,
                RegisterMsgHandler);
            _eventRegisterService.RegisterEvent<EMsgCS, NetMsgHandler>("OnMsg_S2C", "OnMsg_".Length,
                RegisterMsgHandler);
            InitLobby(ServerIp, ServerPort, NetworkDefine.NetKey);
        }

        public override void DoStart(){
            StartLobby();
        }

        public override void DoUpdate(float elapsedMilliseconds){
            _netProxyLobby?.DoUpdate();
            _netProxyRoom?.DoUpdate();
        }

        public override void DoDestroy(){
            _netProxyLobby?.DoDestroy();
            _netProxyRoom?.DoDestroy();
        }

        public void RegisterMsgHandler(EMsgCL type, NetMsgHandler handler){
            _netProxyLobby.RegisterMsgHandler((byte) type, handler);
        }

        public void RegisterMsgHandler(EMsgCS type, NetMsgHandler handler){
            _netProxyRoom.RegisterMsgHandler((byte) type, handler);
        }

        private void InitLobby(string ip, int port, string key){
            _netProxyLobby.OnConnected += OnConnectedLobby;
            _netProxyLobby.Init(ip, port, key);
            //_netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_ReqInit, OnMsg_L2C_ReqInit);
            //_netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_RoomMsg, OnMsg_L2C_RoomMsg);
        }

        public void InitRoom(string ip, int port, string key){
            _netProxyRoom.OnConnected += OnConnectedRoom;
            _netProxyRoom.Init(ip, port, NetworkDefine.NetKey);
            //register msgs

            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_StartGame, OnMsg_S2C_StartGame);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_FrameData, OnMsg_S2C_FrameData);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_RepMissPack,OnNet_ReqInit);
        }


        public void StartRoom(){
            _netProxyRoom.DoStart();
        }


        public void StartLobby(){
            _netProxyLobby.DoStart();
        }


        public void OnConnectedLobby(){
            SendInitMsg();
        }

        public void OnConnectedRoom(){
            Logging.Debug.Log("OnConnected room");
            if (!isReconnected) {
                SendMsgRoom(EMsgCS.C2S_PlayerReady, new Msg_PlayerReady() {roomId = _roomId});
            }
            else {
                EventHelper.Trigger(EEvent.OnRoomGameStart, reconnectedInfo);
            }
        }

        public void SendInput(Msg_PlayerInput msg){
            SendMsgRoom(EMsgCS.C2S_PlayerInput, msg);
        }

        public void SendMsgLobby(EMsgCL msgId, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) msgId);
            body.Serialize(writer);
            _netProxyLobby.Send(Compressor.Compress(writer));
        }

        public void SendMsgRoom(EMsgCS msgId, ISerializable body){
            var writer = new Serializer();
            writer.Put(_playerID);
            writer.Put((byte) msgId);
            body.Serialize(writer);
            _netProxyRoom.Send(Compressor.Compress(writer));
        }

        public void SendMissFrameReq(int missFrameTick){
            Debug.Log($"SendMissFrameReq");
            SendMsgRoom(EMsgCS.C2S_ReqMissFrame,
                new Msg_ReqMissFrame() {startTick = missFrameTick});
        }

        public void SendMissFrameRepAck(int missFrameTick){
            Debug.Log($"SendMissFrameRepAck");
            SendMsgRoom(EMsgCS.C2S_RepMissFrameAck, new Msg_RepMissFrameAck() {missFrameTick = missFrameTick});
        }

        public void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count){
            Msg_HashCode msg = new Msg_HashCode();
            msg.startTick = firstHashTick;
            msg.hashCodes = new long[count];
            for (int i = startIdx; i < count; i++) {
                msg.hashCodes[i] = allHashCodes[i];
            }

            SendMsgRoom(EMsgCS.C2S_HashCode, msg);
        }

        private void OnMsg_S2C_RepMissFrame(Deserializer reader){
            Debug.Log($"OnMsg_S2C_RepMissFrame  RawDataSize" + reader.RawDataSize);
            var msg = reader.Parse<Msg_RepMissFrame>();
            EventHelper.Trigger(EEvent.OnServerMissFrame, msg);
        }

        void OnMsg_L2C_RoomMsg(Deserializer reader){
            var msg = reader.Parse<Msg_CreateRoomResult>();
            _roomId = msg.roomId;
            UnityEngine.Debug.Log("OnMsgLobby_CreateRoom " + msg.port);
            InitRoom(msg.ip, msg.port, NetworkDefine.NetKey);
            StartRoom();
        }

        private object reconnectedInfo;

        void OnMsg_L2C_ReqInit(Deserializer reader){
            var msg = reader.Parse<NetMsg.Lobby.Msg_RepInit>();
            _playerID = msg.playerId;
            Debug.Log("PlayerID " + _playerID + " roomId:" + msg.roomId);
            if (msg.roomId > 0) {
                isReconnected = true;
                InitRoom(msg.ip, msg.port, NetworkDefine.NetKey);
                var subMsg = new Msg_StartGame();
                subMsg.Deserialize(new Deserializer(msg.childMsg));
                reconnectedInfo = subMsg;
                StartRoom();
            }
            else {
                SendCreateRoomMsg();
            }
        }

        void SendInitMsg(){
            SendMsgLobby(EMsgCL.C2L_InitMsg,
                new Msg_RoomInitMsg() {name = "FishMan:" + Application.dataPath.GetHashCode()});
        }

        void SendCreateRoomMsg(){
            UnityEngine.Debug.Log("SendCreateRoomMsg");
            SendMsgLobby(EMsgCL.C2L_CreateRoom, new Msg_CreateRoom() {type = 1, name = "FishManRoom", size = 2});
        }


        private void OnMsg_S2C_FrameData(Deserializer reader){
            var msg = reader.Parse<Msg_ServerFrames>();
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMsg_S2C_StartGame(Deserializer reader){
            var msg = reader.Parse<Msg_StartGame>();
            _roomId = msg.RoomID;
            Debug.Log($"Starting simulation. Total actors: {msg.AllActors.Length}. Local ActorID: {msg.ActorID}");
            EventHelper.Trigger(EEvent.OnRoomGameStart, msg);
        }

        public void OnMsg_S2C_LoadingProgress(Deserializer reader){
            var msg = reader.Parse<Msg_AllLoadingProgress>();
            var isDone = msg.isAllDone;
            if (isDone) {
                EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
            }
            else {
                EventHelper.Trigger(EEvent.OnLoadingProgress, msg);
            }
        }

        public void OnEvent_LoadMapDone(object param){
            var level = (int) param;
            _constStateService.curLevel = level;
            Debug.Log($"hehe OnEvent_LoadMapDone isReconnected {isReconnected} ");
            if (isReconnected) {
                EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
            }
            else {
                SendMsgRoom(EMsgCS.C2S_LoadingProgress, new Msg_LoadingProgress() {progress = 100});
            }
        }
    }
}