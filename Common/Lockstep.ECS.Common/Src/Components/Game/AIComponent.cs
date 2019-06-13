using System;
using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class AIComponent : IComponent {
        public LFloat timer;
        public LFloat updateInterval;
        public LFloat fireRate;

    }
}
