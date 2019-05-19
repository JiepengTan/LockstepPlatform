using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public class AIComponent : IComponent {
        public LFloat timer;
        public LFloat updateInterval;
        public LFloat fireRate;

    }
}