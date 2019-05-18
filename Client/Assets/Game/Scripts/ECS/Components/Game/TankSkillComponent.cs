using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{

    [Game]
    public class TankSkillComponent : IComponent {
        public LFloat cd;
        public LFloat timer;
        public int BulletType;
    }
}