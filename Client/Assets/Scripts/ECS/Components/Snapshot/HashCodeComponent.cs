using Entitas;

namespace Lockstep.ECS.Snapshot
{
    [Snapshot]
    public sealed class HashCodeComponent : IComponent
    {
        public long value;
    }
}