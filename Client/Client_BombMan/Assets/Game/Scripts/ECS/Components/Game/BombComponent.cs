using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class BombComponent : IComponent {
        public int damageRange;
        public LFloat timer;
    }
}