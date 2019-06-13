using System.Collections;
using LitJson;
using Lockstep.Client;
using NetMsg.Common;
using Lockstep.Util;

namespace Test {
    public class DebugGameMsgHandler : BaseRoomMsgHandler {
        private RoomMsgManager _roomMsgManager;
        public DebugGameMsgHandler(RoomMsgManager rmm){
            _roomMsgManager = rmm;
        }
        public override void OnServerFrames(Msg_ServerFrames msg){ }
        public override void OnMissFrames(Msg_RepMissFrame msg){ }
        public override void OnGameEvent(byte[] data){ }
        public override void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }

        public override void OnLoadingProgress(byte[] progresses){
            //Log("OnLoadingProgress " + JsonMapper.ToJson(progresses));
        }

        public override void OnAllFinishedLoaded(short level){
            Log("OnAllFinishedLoaded " + level);
        }

        IEnumerator YiledLoadingMap(){
            int i = 0;
            while (i++ <= 20) {
                yield return new WaitForSeconds(0.1f);
                _roomMsgManager.OnLevelLoadProgress(i * 0.05f);
            }
        }

        public override void OnTcpHello(Msg_G2C_Hello msg){
            Log($"OnTcpHello msg:{msg} ");
            CoroutineHelper.StartCoroutine(YiledLoadingMap());
        }

        public override void OnUdpHello(int mapId, byte localId){
            Log($"OnUdpHello mapId:{mapId} localId:{localId}");
        }

        public override void OnGameStartFailed(){
            Log($"OnGameStartFailed");
        }
    }
}