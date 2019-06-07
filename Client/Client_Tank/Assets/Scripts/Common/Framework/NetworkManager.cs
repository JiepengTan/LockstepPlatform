using System.Collections.Generic;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Common;

using UnityEngine;

namespace Lockstep.Game {
    public partial class NetworkManager : SingletonManager<NetworkManager>, INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 9050;

        private BaseNetProxy _netProxyLobby;
        private BaseNetProxy _netProxyRoom;

        private long _playerID;
        private int _roomId;

        private bool IsReconnected = false; //是否是重连
        public int Ping => _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected => _netProxyLobby != null && _netProxyLobby.Connected;

        public override void DoAwake(IServiceContainer services){
            //_netProxyRoom = new BaseNetProxy((int) EMsgSC.EnumCount);
            //_netProxyLobby = new BaseNetProxy((int) EMsgCL.EnumCount);
            //_eventRegisterService.RegisterManagersEvent<EMsgCL, NetMsgHandler>("OnMsg_L2C", "OnMsg_".Length,
            //    RegisterMsgHandler);
            //_eventRegisterService.RegisterManagersEvent<EMsgSC, NetMsgHandler>("OnMsg_S2C", "OnMsg_".Length,
            //    RegisterMsgHandler);
            //Editor mode don't need network
            if (_constStateService.IsVideoMode) return;
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

       //public void RegisterMsgHandler(EMsgLC type, NetMsgHandler handler){
       //    _netProxyLobby.RegisterMsgHandler((byte) type, handler);
       //}

        public void RegisterMsgHandler(EMsgSC type, NetMsgHandler handler){
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
            //Register msgs

            //_netProxyRoom.RegisterMsgHandler((byte) EMsgSC.S2C_StartGame, OnMsg_S2C_StartGame);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgSC.S2C_FrameData, OnMsg_S2C_FrameData);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgSC.S2C_RepMissPack,OnNet_ReqInit);
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
            if (!IsReconnected) {
               // SendMsgRoom(EMsgSC.C2S_PlayerReady, new NetMsg.Lobby.Msg_PlayerReady { });
            }
            else {
                EventHelper.Trigger(EEvent.OnRoomGameStart, reconnectedInfo);
            }
        }

        public void SendInput(Msg_PlayerInput msg){
           // SendMsgRoom(EMsgSC.C2S_PlayerInput, msg);
        }

        public void SendMsgLobby(EMsgSC msgId, ISerializable body){
            var writer = new Serializer();
            writer.PutByte((byte) msgId);
            body.Serialize(writer);
            _netProxyLobby.Send(Compressor.Compress(writer));
        }

        public void SendMsgRoom(EMsgSC msgId, ISerializable body){
            var writer = new Serializer();
            writer.PutInt64(_playerID);
            writer.PutByte((byte) msgId);
            body.Serialize(writer);
            _netProxyRoom.Send(Compressor.Compress(writer));
        }

        public void SendMissFrameReq(int missFrameTick){
            Debug.Log($"SendMissFrameReq");
           // SendMsgRoom(EMsgSC.C2S_ReqMissFrame,
            //    new Msg_ReqMissFrame() {startTick = missFrameTick});
        }

        public void SendMissFrameRepAck(int missFrameTick){
            Debug.Log($"SendMissFrameRepAck");
           // SendMsgRoom(EMsgSC.C2S_RepMissFrameAck, new Msg_RepMissFrameAck() {missFrameTick = missFrameTick});
        }

        public void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count){
           // Msg_HashCode msg = new Msg_HashCode();
           // msg.startTick = firstHashTick;
           // msg.hashCodes = new long[count];
           // for (int i = startIdx; i < count; i++) {
           //     msg.hashCodes[i] = allHashCodes[i];
           // }
//
           // SendMsgRoom(EMsgSC.C2S_HashCode, msg);
        }

        private void OnMsg_S2C_RepMissFrame(Deserializer reader){
            Debug.Log($"OnMsg_S2C_RepMissFrame  RawDataSize" + reader.RawDataSize);
            var msg = reader.Parse<Msg_RepMissFrame>();
            EventHelper.Trigger(EEvent.OnServerMissFrame, msg);
        }

        void OnMsg_L2C_RoomMsg(Deserializer reader){
           // var msg = reader.Parse<Msg_CreateRoomResult>();
           // _roomId = msg.roomId;
           // UnityEngine.Debug.Log("OnMsgLobby_CreateRoom " + 32);
           // InitRoom("127", 32, NetworkDefine.NetKey);
           // StartRoom();
        }

        private object reconnectedInfo;

        void OnMsg_L2C_RepLogin(Deserializer reader){
           //var msg = reader.Parse<NetMsg.Lobby.Msg_RepLogin>();
           //_playerID = msg.playerId;
           //Debug.Log("PlayerID " + _playerID + " roomId:" + msg.roomId);
           //if (msg.roomId > 0) {
           //    IsReconnected = true;
           //    InitRoom(msg.ip, msg.port, NetworkDefine.NetKey);
           //    var subMsg = new Msg_StartGame();
           //    subMsg.Deserialize(new Deserializer(msg.childMsg));
           //    reconnectedInfo = subMsg;
           //    StartRoom();
           //}
           //else {
           //    EventHelper.Trigger(EEvent.OnLoginResult, msg);
           //}
        }

        void SendInitMsg(){
           // SendMsgLobby(EMsgCL.C2L_ReqLogin,
           //     new Msg_RoomInitMsg() {name = "FishMan:" + Application.dataPath.GetHashCode()});
        }

        void SendCreateRoomMsg(){
            UnityEngine.Debug.Log("SendCreateRoomMsg");
           // SendMsgLobby(EMsgCL.C2L_CreateRoom, new Msg_CreateRoom() {type = 1, name = "FishManRoom", size = 2});
        }


        private void OnMsg_S2C_FrameData(Deserializer reader){
            var msg = reader.Parse<Msg_ServerFrames>();
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMsg_S2C_StartGame(Deserializer reader){
           // var msg = reader.Parse<Msg_StartRoomGame>();
           // _roomId = msg.RoomID;
           // Debug.Log($"Starting simulation. Total actors: {msg.AllActors.Length}. Local ActorID: {msg.ActorID}");
           // EventHelper.Trigger(EEvent.OnRoomGameStart, msg);
        }

        public void OnMsg_S2C_LoadingProgress(Deserializer reader){
           // var msg = reader.Parse<Msg_AllLoadingProgress>();
           // var isDone = msg.isAllDone;
           // if (isDone) {
           //     EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
           // }
           // else {
           //     EventHelper.Trigger(EEvent.OnLoadingProgress, msg);
           // }
        }

        public void OnEvent_LoadMapDone(object param){
            var level = (int) param;
            _constStateService.curLevel = level;
            Debug.Log($"OnEvent_LoadMapDone isReconnected {IsReconnected}  isPlaying:{Application.isPlaying} ");
            if (IsReconnected || _constStateService.IsVideoMode) {
                EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
            }
            else {
               // SendMsgRoom(EMsgSC.C2S_LoadingProgress, new Msg_LoadingProgress() {progress = 100});
            }
        }
    }
}