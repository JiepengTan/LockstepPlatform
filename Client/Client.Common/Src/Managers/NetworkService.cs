
using System.Collections.Generic;
using LitJson;
using NetMsg.Common;
using Lockstep.Client;
using Lockstep.Logging;
using Lockstep.Util;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public partial class NetworkService : BaseService, INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 7250;
        private LoginManager _loginMgr;
        private Client.RoomMsgManager _roomMsgMgr;

        private long _playerID;
        private int _roomId;

        private bool IsReconnected = false; //是否是重连
        public int Ping => 50;//=> _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected = true; // => _netProxyLobby != null && _netProxyLobby.Connected;
        public RoomInfo[] RoomInfos => _loginMgr.RoomInfos;
        public List<RoomPlayerInfo> PlayerInfos => _loginMgr.PlayerInfos;

        private bool _isVideoMode;
        public override void DoAwake(IServiceContainer services){
            _isVideoMode = _constStateService.IsVideoMode;
            if (_isVideoMode) return;
            _roomMsgMgr = new Client.RoomMsgManager();
            _loginMgr = new LoginManager();
            _roomMsgMgr.Init(this);
            _loginMgr.Init(_roomMsgMgr, this, ServerIp, (ushort) ServerPort);
            _loginMgr.DoAwake();
        }

        public override void DoStart(){
            if (_isVideoMode) return;
            Utils.StartServices();
            _loginMgr.DoStart();
        }

        public override void DoUpdate(int deltaTimeMs){
            if (_isVideoMode) return;
            Utils.UpdateServices();
            _roomMsgMgr?.DoUpdate(deltaTimeMs);
            _loginMgr?.DoUpdate(deltaTimeMs);
        }


        public override void DoDestroy(){
            if (_isVideoMode) return;
            _loginMgr = null;
            _roomMsgMgr = null;
            _loginMgr?.DoDestroy();
            _roomMsgMgr?.DoDestroy();
        }
        

        public void OnEvent_TryLogin(object param){
            if (_isVideoMode) return;
            Debug.Log("OnEvent_TryLogin" + param.ToJson());
            var loginInfo = param as LoginParam;
            var _account = loginInfo.account;
            var _password = loginInfo.password;
            _loginMgr.Login(_account, _password);
        }

        private void OnEvent_OnConnectToGameServer(object param){
            if (_isVideoMode) return;
            var isReconnect = (bool) param;
            _constStateService.IsReconnecting = isReconnect;
        }

        private void OnEvent_LevelLoadProgress(object param){
            if (_isVideoMode) return;
            _roomMsgMgr.OnLevelLoadProgress((float) param);
            CheckLoadingProgress();
        }

        private void OnEvent_PursueFrameProcess(object param){
            if (_isVideoMode) return;
            _roomMsgMgr.FramePursueRate = (float) param;
            CheckLoadingProgress();
        }

        private void OnEvent_PursueFrameDone(object param){
            if (_isVideoMode) return;
            _roomMsgMgr.FramePursueRate = 1;
            CheckLoadingProgress();
        }

        void CheckLoadingProgress(){
            if (_roomMsgMgr.IsReconnecting) {
                var curProgress = _roomMsgMgr.CurProgress / 100.0f;
                EventHelper.Trigger(EEvent.ReconnectLoadProgress, curProgress);
                if (_roomMsgMgr.CurProgress >= 100) {
                    _constStateService.IsReconnecting = false;
                    _roomMsgMgr.IsReconnecting = false;
                    EventHelper.Trigger(EEvent.ReconnectLoadDone);
                }
            }
        }
    }

    public partial class NetworkService  {
        
        #region Login Handler
        public void CreateRoom(int mapId, string name, int size){
            _loginMgr.CreateRoom(mapId, name, size);
        }

        public void StartGame(){
            _loginMgr.StartGame();
        }

        public void ReadyInRoom(bool isReady){
            _loginMgr.ReadyInRoom(isReady);
        }

        public void JoinRoom(int roomId){
            _loginMgr.JoinRoom(roomId, (infos) => { EventHelper.Trigger(EEvent.OnJoinRoomResult, infos); });
        }

        public void ReqRoomList(int startIdx){
            _loginMgr.ReqRoomList(startIdx);
        }

        public void LeaveRoom(){
            _loginMgr.LeaveRoom();
        }


        public void SendChatInfo(RoomChatInfo chatInfo){
            _loginMgr.SendChatInfo(chatInfo);
        }

        #endregion

        #region Room Msg Handler

        public void SendGameEvent(byte[] data){
            if (_isVideoMode) return;
            _roomMsgMgr.SendGameEvent(data);
        }

        public void SendInput(Msg_PlayerInput msg){
            if (_isVideoMode) return;
            _roomMsgMgr.SendInput(msg);
        }

        public void SendMissFrameReq(int missFrameTick){
            if (_isVideoMode) return;
            _roomMsgMgr.SendMissFrameReq(missFrameTick);
        }

        public void SendMissFrameRepAck(int missFrameTick){
            if (_isVideoMode) return;
            _roomMsgMgr.SendMissFrameRepAck(missFrameTick);
        }

        public void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count){
            if (_isVideoMode) return;
            _roomMsgMgr.SendHashCodes(firstHashTick, allHashCodes, startIdx, count);
        }

        #endregion
    }

    public partial class NetworkService : IRoomMsgHandler, ILoginHandler {
        public static NetworkService Instance { get; private set; }
        public NetworkService(){
            Instance = this;
        }

        void Log(string msg){
            Debug.Log(msg);
        }
        void ILoginHandler.SetLogger(DebugInstance debug){ }
        void IRoomMsgHandler.SetLogger(DebugInstance debug){ }

        public void OnConnectedLoginServer(){
            EventHelper.Trigger(EEvent.OnConnLogin);
            Log("OnConnLogin ");
        }

        public void OnLoginFailed(ELoginResult result){
            Log("Login failed reason " + result);
            EventHelper.Trigger(EEvent.OnLoginFailed, result);
        }

        public void OnConnLobby(RoomInfo[] roomInfos){
            EventHelper.Trigger(EEvent.OnLoginResult, roomInfos);
        }

        public void OnRoomInfo(RoomInfo[] roomInfos){
            Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            EventHelper.Trigger(EEvent.OnRoomInfoUpdate, roomInfos);
        }


        public void OnCreateRoom(RoomInfo roomInfo, RoomPlayerInfo[] playerInfos){
            if (roomInfo == null)
                Log("CreateRoom failed reason ");
            else {
                Log("CreateRoom " + roomInfo.ToString());
                EventHelper.Trigger(EEvent.OnCreateRoom, roomInfo);
            }
        }

        public void OnStartRoomResult(int reason){
            if (reason != 0) {
                Log("StartGame failed reason " + reason);
            }
        }

        public void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd, bool isReconnect){
            Log("OnGameStart " + msg + " tcpEnd " + tcpEnd);
            EventHelper.Trigger(EEvent.OnConnectToGameServer, isReconnect);
        }

        public void OnPlayerJoinRoom(RoomPlayerInfo info){
            EventHelper.Trigger(EEvent.OnPlayerJoinRoom, info);
        }

        public void OnPlayerLeaveRoom(long userId){
            EventHelper.Trigger(EEvent.OnPlayerLeaveRoom, userId);
        }

        public void OnRoomChatInfo(RoomChatInfo info){
            EventHelper.Trigger(EEvent.OnRoomChatInfo, info);
        }

        public void OnPlayerReadyInRoom(long userId, byte state){
            EventHelper.Trigger(EEvent.OnPlayerReadyInRoom, new object[] {userId, state});
        }

        public void OnLeaveRoom(){
            EventHelper.Trigger(EEvent.OnLeaveRoom);
        }

        public void OnTickPlayer(byte reason){
            EventHelper.Trigger(EEvent.OnTickPlayer, reason);
        }

        public void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos){ }

        public void OnTcpHello(Msg_G2C_Hello msg){
            Log($"OnTcpHello msg:{msg} ");
            EventHelper.Trigger(EEvent.OnGameCreate, msg);
            //CoroutineHelper.StartCoroutine(YiledLoadingMap());
        }

        public void OnUdpHello(int mapId, byte localId){
            Log($"OnUdpHello mapId:{mapId} localId:{localId}");
        }

        public void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }

        public void OnGameStartFailed(){
            Log($"OnGameStartFailed");
        }

        public void OnLoadingProgress(byte[] progresses){
            EventHelper.Trigger(EEvent.OnLoadingProgress, progresses);
        }

        public void OnAllFinishedLoaded(short level){
            Log("OnAllFinishedLoaded " + level);
            EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, level);
        }

        public void OnServerFrames(Msg_ServerFrames msg){
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMissFrames(Msg_RepMissFrame msg){
            Log("OnMissFrames");
            if (msg == null) return;
            EventHelper.Trigger(EEvent.OnServerMissFrame, msg);
        }

        public void OnGameEvent(byte[] data){ }
    }
}