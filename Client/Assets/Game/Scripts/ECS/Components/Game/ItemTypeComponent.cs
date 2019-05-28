using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    public enum EItemType {
        Boom,
        AddLife,
        Upgrade,
    }

    [Game]
    [System.Serializable]
    public partial class ItemTypeComponent : IComponent {
        public EItemType type;
        public byte killerActorId;
    }
}