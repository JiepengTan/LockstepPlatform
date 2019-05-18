using System.Collections.Generic;
using Lockstep.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lockstep.Game {
    public partial class Main {
        private void DoAwake(){ 
            Screen.SetResolution(1024, 768, false);
        }
        
        private void DoStart(){
            int maxPlayerCount = 2;
#if UNITY_STANDALONE || UNITY_STANDALONE_OSX
            maxPlayerCount = 2;
#else //手游 只有一个输入端
            maxPlayerCount = 1;
#endif
            for (int i = 0; i < maxPlayerCount; i++) {
                var info = new PlayerInfo();
                info.remainPlayerLife = 3;
                info.score = 0;
                //.allPlayerInfos[i] = info;
            }

            //levelMgr.LoadGame(1);
        }

        private  void DoUpdate(float deltaTime){ }
        private  void DoFixedUpdate(){ }
        private  void DoDestroy(){ }
        
        public bool IsGameOver(){
            return false;
        }
    }
}