using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Common;
using Lockstep.Client;
using Lockstep.Networking;
using Lockstep.Util;
using UnityEngine;

namespace Lockstep.Game {
    public class LoginHandler : BaseLoginHandler {
        public override void OnConnectedLoginServer(){
            Log("OnConnLogin ");
        }

        public override void OnLoginFailed(ELoginResult result){
            Log("Login failed reason " + result);
            EventHelper.Trigger(EEvent.OnLoginFailed, result);
        }

        public override void OnConnLobby(RoomInfo[] roomInfos){
            EventHelper.Trigger(EEvent.OnLoginResult, roomInfos);
        }

        public override void OnRoomInfo(RoomInfo[] roomInfos){
            Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            EventHelper.Trigger(EEvent.OnRoomInfoUpdate, roomInfos);
        }


        public override void OnCreateRoom(RoomInfo roomInfo, RoomPlayerInfo[] playerInfos){
            if (roomInfo == null)
                Log("CreateRoom failed reason ");
            else {
                Log("CreateRoom " + roomInfo.ToString());
                EventHelper.Trigger(EEvent.OnCreateRoom, roomInfo);
            }
        }

        public override void OnStartRoomResult(int reason){
            if (reason != 0) {
                Log("StartGame failed reason " + reason);
            }
        }

        public override void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd,bool isReconnect){
            Log("OnGameStart " + msg + " tcpEnd " + tcpEnd);
            EventHelper.Trigger(EEvent.OnConnectToGameServer,isReconnect);
        }

        public override void OnPlayerJoinRoom(RoomPlayerInfo info){
            EventHelper.Trigger(EEvent.OnPlayerJoinRoom, info);
        }

        public override void OnPlayerLeaveRoom(long userId){
            EventHelper.Trigger(EEvent.OnPlayerLeaveRoom, userId);
        }

        public override void OnRoomChatInfo(RoomChatInfo info){
            EventHelper.Trigger(EEvent.OnRoomChatInfo, info);
        }

        public override void OnPlayerReadyInRoom(long userId, byte state){
            EventHelper.Trigger(EEvent.OnPlayerReadyInRoom, new object[] {userId, state});
        }

        public override void OnLeaveRoom(){
            EventHelper.Trigger(EEvent.OnLeaveRoom);
        }

        public override void OnTickPlayer(byte reason){
            EventHelper.Trigger(EEvent.OnTickPlayer, reason);
        }

        public override void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos){ }
    }

    public class GameMsgHandler : BaseRoomMsgHandler {
        public override void OnTcpHello(Msg_G2C_Hello msg){
            Log($"OnTcpHello msg:{msg} ");
            EventHelper.Trigger(EEvent.OnGameCreate, msg);
            //CoroutineHelper.StartCoroutine(YiledLoadingMap());
        }

        public override void OnUdpHello(int mapId, byte localId){
            Log($"OnUdpHello mapId:{mapId} localId:{localId}");
        }

        public override void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }

        public override void OnGameStartFailed(){
            Log($"OnGameStartFailed");
        }

        public override void OnLoadingProgress(byte[] progresses){
            EventHelper.Trigger(EEvent.OnLoadingProgress, progresses);
        }

        public override void OnAllFinishedLoaded(short level){
            Log("OnAllFinishedLoaded " + level);
            EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, level);
        }

        public override void OnServerFrames(Msg_ServerFrames msg){
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public override void OnMissFrames(Msg_RepMissFrame msg){
            Log("OnMissFrames");
            if (msg == null) return;
            EventHelper.Trigger(EEvent.OnServerMissFrame, msg);
        }

        public override void OnGameEvent(byte[] data){ }
    }

    public class LoginParam {
        public string password;
        public string account;
    }

    public static class JsonExtension {
        public static string ToJson(this object obj){
            return obj == null ? "null" : JsonMapper.ToJson(obj);
        }
    }


    public partial class NetworkManager : SingletonManager<NetworkManager>, INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 7250;
        private LoginManager _loginMgr;
        private RoomMsgManager _roomMsgMgr;

        private long _playerID;
        private int _roomId;

        private bool IsReconnected = false; //是否是重连
        public int Ping { get; set; } //=> _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected; // => _netProxyLobby != null && _netProxyLobby.Connected;

        public RoomInfo[] RoomInfos => _loginMgr.RoomInfos;
        public List<RoomPlayerInfo> PlayerInfos => _loginMgr.PlayerInfos;

        public override void DoAwake(IServiceContainer services){
            if (_constStateService.IsVideoMode) return;
            _roomMsgMgr = new RoomMsgManager();
            _roomMsgMgr.Init(new GameMsgHandler());
            _loginMgr = new LoginManager();
            _loginMgr.Init(_roomMsgMgr, new LoginHandler(), ServerIp, (ushort) ServerPort);
            _loginMgr.DoAwake();
        }

        public override void DoStart(){
            Utils.StartServices();
            _loginMgr.DoStart();
        }

        public override void DoUpdate(float elapsedMilliseconds){
            Utils.UpdateServices();
            var deltaTime = (int) elapsedMilliseconds;
            _roomMsgMgr.DoUpdate(deltaTime);
            _loginMgr.DoUpdate(deltaTime);
        }


        public void OnEvent_TryLogin(object param){
            Debug.Log("OnEvent_TryLogin" + param.ToJson());
            var loginInfo = param as LoginParam;
            var _account = loginInfo.account;
            var _password = loginInfo.password;
            _loginMgr.Login(_account, _password);
        }
        public void OnEvent_OnConnectToGameServer(object param){
            var isReconnect = (bool) param;
            _constStateService.IsReconnecting = isReconnect;
        }
        public void OnEvent_LoadLevelProgress(object param){
            _roomMsgMgr.OnLoadLevelProgress((float) param);
            CheckLoadingProgress();
        }

        public void OnEvent_PursueFrameProcess(object param){
            _roomMsgMgr.FramePursueRate = (float) param;
            CheckLoadingProgress();
        }

        public void OnEvent_PursueFrameDone(object param){
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

        public override void DoDestroy(){
            _loginMgr?.DoDestroy();
            _roomMsgMgr?.DoDestroy();
            _loginMgr = null;
            _roomMsgMgr = null;
        }


        private object reconnectedInfo;


        // public void OnEvent_LoadLevelDone(object param){
        //     var level = (int) param;
        //     _constStateService.curLevel = level;
        //     Debug.Log($"OnEvent_LoadLevelDone isReconnected {IsReconnected}  isPlaying:{Application.isPlaying} ");
        //     if (IsReconnected || _constStateService.IsVideoMode) {
        //         EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
        //     }
        //     else {
        //         // SendMsgRoom(EMsgSC.C2S_LoadingProgress, new Msg_LoadingProgress() {progress = 100});
        //     }
        // }

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
            _roomMsgMgr.SendGameEvent(data);
        }

        public void SendInput(Msg_PlayerInput msg){
            _roomMsgMgr.SendInput(msg);
        }

        public void SendMissFrameReq(int missFrameTick){
            _roomMsgMgr.SendMissFrameReq(missFrameTick);
        }

        public void SendMissFrameRepAck(int missFrameTick){
            _roomMsgMgr.SendMissFrameRepAck(missFrameTick);
        }

        public void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count){
            _roomMsgMgr.SendHashCodes(firstHashTick, allHashCodes, startIdx, count);
        }

        #endregion
    }
}