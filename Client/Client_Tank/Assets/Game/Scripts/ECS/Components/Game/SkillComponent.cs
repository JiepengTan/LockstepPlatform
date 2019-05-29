using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{

    [Game]
    [System.Serializable]
    public partial class SkillComponent : IComponent {
        public LFloat cd;
        /// <=0 表示cd 冷却
        public LFloat cdTimer;
        public int bulletId;
        public bool isNeedFire;
    }
}