using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    public class TankAIComponent : IComponent {
        public LFloat timer;
        public LFloat updateInterval;
        public LFloat fireRate;

    }
}