using System;
using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
    [Game]
    [System.Serializable]
    public class DelayCallComponent :IComponent {
        public LFloat delayTimer;
        public Action callBack;
    }
}