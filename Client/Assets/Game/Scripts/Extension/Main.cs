using System.Collections.Generic;
using Lockstep.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lockstep.Game {
    public partial class Main {
        private void DoAwake(){
            Screen.SetResolution(1024, 768, false);
        }
        private void DoStart(){ }
        private void DoUpdate(float deltaTime){ }
        private void DoFixedUpdate(){ }
        private void DoDestroy(){ }
        public bool IsGameOver(){
            return false;
        }
    }
}