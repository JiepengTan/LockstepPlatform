using System;
using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
    [Game]
    public partial class DelayCallComponent : IComponent {
        public LFloat delayTimer;
        public int callBack;
    }

}