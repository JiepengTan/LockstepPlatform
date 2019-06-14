using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{
    [Game]
    public partial class MoveComponent : IComponent {
        public LFloat moveSpd;
        public LFloat maxMoveSpd;
        public bool isChangedDir;
    }
}