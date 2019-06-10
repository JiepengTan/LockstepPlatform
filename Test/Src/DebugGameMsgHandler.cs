using System.Collections;
using LitJson;
using Lockstep.Client;
using NetMsg.Common;
using Lockstep.Util;

namespace Test {
    public class DebugGameMsgHandler : BaseRoomMsgHandler {
        public override void OnServerFrames(Msg_ServerFrames msg){ }
        public override void OnMissFrames(Msg_ServerFrames msg){ }
        public override void OnGameEvent(byte[] data){ }
        public override void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }

        public override void OnLoadingProgress(byte[] progresses){
            Log("OnLoadingProgress " + JsonMapper.ToJson(progresses));
        }

        public override void OnAllFinishedLoaded(short level){
            Log("OnAllFinishedLoaded " + level);
        }
        public override void OnGameInfo(Msg_G2C_GameStartInfo msg){
            Log($"OnUdpHello msg:{msg} ");
        }

        IEnumerator YiledLoadingMap(){
            int i = 0;
            while (i++ <= 20) {
                yield return new WaitForSeconds(0.1f);
                this._mgr.OnLoadLevelProgress(i * 0.05f);
            }
        }

        public override void OnTcpHello(int mapId, byte localId){
            Log($"OnTcpHello mapId:{mapId} localId:{localId}");
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