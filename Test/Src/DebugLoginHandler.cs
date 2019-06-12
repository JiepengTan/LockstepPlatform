using System;
using LitJson;
using Lockstep.Client;
using NetMsg.Common;

namespace Test {
    
    public class DebugLoginHandler : BaseLoginHandler {
        private LoginManager _loginMgr;

        public DebugLoginHandler(LoginManager lm){
            _loginMgr = lm;
        }
        public int RandomSeed = 1167;
        public override void OnConnectedLoginServer(){
            Log("OnConnLogin ");
            var _account = "FakeClient " +  new Random(RandomSeed).Next();
            var  _password = "1234";
            _loginMgr.Login(_account, _password);
        }
            
        public override void OnConnLobby(RoomInfo[] roomInfos){
            Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            if (roomInfos == null) {
                _loginMgr.CreateRoom(3, "DebugTestRoom", 2);
                return;
            }
        }

        public override void OnPlayerJoinRoom(RoomPlayerInfo info){
            Log("OnPlayerJoinRoom  " + info);
            Log("SomeOne joined so: StartGame  ");
            _loginMgr.StartGame();
        }

        public override void OnCreateRoom(RoomInfo roomInfo,RoomPlayerInfo[] playerInfos){
            if (roomInfo == null)
                Log("CreateRoom failed reason ");
            else {
                Log("CreateRoom " + roomInfo.ToString());
            }
        }

        public override void OnStartRoomResult(int reason){
            if (reason != 0) {
                Log("StartGame failed reason " + reason);
            }
        }

    }
}