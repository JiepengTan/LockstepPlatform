using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{

    [Game]
    public partial class SkillComponent : IComponent {
        public LFloat cd;
        /// <=0 表示cd 冷却
        public LFloat cdTimer;
        public int bulletId;
        public bool isNeedFire;
    }
}