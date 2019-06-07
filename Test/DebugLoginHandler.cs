using System;
using LitJson;
using Lockstep.Client;
using NetMsg.Common;

namespace Test {
    
    public class DebugLoginHandler : BaseLoginHandler {
        public override void OnConnectedLoginServer(){
            _loginMgr.Log("OnConnLogin ");
            var _account = "FakeClient " + new Random(1154).Next();
            var  _password = "1234";
            _loginMgr.Login(_account, _password);
        }
            
        public override void OnRoomInfo(RoomInfo[] roomInfos){
            _loginMgr.Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            if (roomInfos == null) {
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
}