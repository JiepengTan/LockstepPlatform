using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lockstep.Logging;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public partial class Main {
        private void DoAwake(){
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