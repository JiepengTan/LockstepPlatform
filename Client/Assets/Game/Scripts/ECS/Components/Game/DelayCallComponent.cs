using System;
using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
    [Game]
    [System.Serializable]
    public class DelayCallComponent :IComponent {
        public LFloat timer;
        public LFloat delay;
        public Action callBack;
    }
}