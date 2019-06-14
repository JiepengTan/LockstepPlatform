using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{
    public enum EItemType {
        Boom,
        AddLife,
        Upgrade,
    }

    [Game]
    public partial class ItemTypeComponent : IComponent {
        public EItemType type;
        public byte killerActorId;
    }
}