using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class MoveComponent : IComponent {
        public LFloat moveSpd;
        public LFloat maxMoveSpd;
        public EDir dir;
    }
}