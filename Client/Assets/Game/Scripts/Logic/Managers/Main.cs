using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lockstep.Game {
    public partial class Main {
        private RandomService _random = new RandomService();
        private void DoAwake(){
            //register other services whoa not a monoBehaviour
            RegisterService(_random);
            //set resolution for debug
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