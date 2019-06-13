using System;
using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
    [Game]
    [System.Serializable]
    public partial class DelayCallComponent : IComponent {
        public LFloat delayTimer;
        public Action callBack;
    }

}