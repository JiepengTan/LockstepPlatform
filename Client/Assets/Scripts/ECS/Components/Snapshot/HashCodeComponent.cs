using Entitas;

namespace Lockstep.ECS.Snapshot
{
    [Snapshot]
    public partial class HashCodeComponent : IComponent
    {
        public long value;
    }
}