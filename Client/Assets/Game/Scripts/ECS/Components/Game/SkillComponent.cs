using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{

    [Game]
    [System.Serializable]
    public class SkillComponent : IComponent {
        public LFloat cd;
        public LFloat timer;
        public EAssetID bulletId;
        public bool isNeedFire;
    }
}