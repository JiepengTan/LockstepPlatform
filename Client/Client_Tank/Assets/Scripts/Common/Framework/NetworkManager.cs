using System.Collections.Generic;
using LitJson;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Common;
using Lockstep.Client;
using UnityEngine;

namespace Lockstep.Game {
    public class LoginHandler : BaseLoginHandler {
        public override void OnConnectedLoginServer(){
            _loginMgr.Log("OnConnLogin ");
            var _account = "FakeClient " + (int) (Random.value * 10000);
            var _password = "1234";
            _loginMgr.Login(_account, _password);
        }

        public override void OnRoomInfo(RoomInfo[] roomInfos){
            _loginMgr.Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            if (roomInfos == null || roomInfos.Length < 3) {
                _loginMgr.CreateRoom(3, "TestRoom", 1);
                return;
            }
        }


        public override void OnCreateRoom(RoomInfo roomInfo){
            if (roomInfo == null)
                _loginMgr.Log("CreateRoom failed reason ");
            else {
                _loginMgr.Log("CreateRoom " + roomInfo.ToString());
                _loginMgr.StartGame();
            }
        }

        public override void OnStartRoomResult(int reason){
            if (reason != 0) {
                _loginMgr.Log("StartGame failed reason " + reason);
            }
        }

        public override void OnGameStart(int mapId, byte localId){
            _loginMgr.Log("mapId" + mapId + " localId" + localId);
        }
    }

    public partial class NetworkManager : SingletonManager<NetworkManager>, INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 7250;
        private LoginManager _loginMgr;
        private LoginHandler _loginHandler;
        private long _playerID;
        private int _roomId;

        private bool IsReconnected = false; //是否是重连
        public int Ping { get; set; } //=> _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected; // => _netProxyLobby != null && _netProxyLobby.Connected;

        public override void DoAwake(IServiceContainer services){
            if (_constStateService.IsVideoMode) return;
            _loginHandler = new LoginHandler();
            _loginMgr = new LoginManager();
            _loginMgr.Init(_loginHandler, ServerIp, (ushort) ServerPort);
            _loginMgr.DoAwake();
        }

        public override void DoStart(){
            _loginMgr.DoStart();
        }

        public override void DoUpdate(float elapsedMilliseconds){
            _loginMgr.DoUpdate((int) elapsedMilliseconds);
        }

        public override void DoDestroy(){ }


        public void OnConnectedLobby(){
            SendInitMsg();
        }

        public void OnConnectedRoom(){
            Logging.Debug.Log("OnConnected room");
            if (!IsReconnected) {
                // SendMsgRoom(EMsgSC.C2S_PlayerReady, new NetMsg.Lobby.Msg_PlayerReady { });
            }
            else { }
        }

        public void SendInput(Msg_PlayerInput msg){
            // SendMsgRoom(EMsgSC.C2S_PlayerInput, msg);
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

        public void SendMsgLobby(EMsgSC msgId, ISerializable body){ }

        public void SendMsgRoom(EMsgSC msgId, ISerializable body){
            //var writer = new Serializer();
            //writer.PutInt64(_playerID);
            //writer.PutByte((byte) msgId);
            //body.Serialize(writer);
            //_netProxyRoom.Send(Compressor.Compress(writer));
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