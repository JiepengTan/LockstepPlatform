using Entitas;

namespace Lockstep.ECS.Debug
{
    [Debugging]
    public sealed class HashCodeComponent : IComponent
    {
        public long value;
    }
}