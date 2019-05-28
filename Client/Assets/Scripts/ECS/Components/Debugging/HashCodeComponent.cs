using Entitas;

namespace Lockstep.ECS.Debug
{
    [Debugging]
    public partial class HashCodeComponent : IComponent
    {
        public long value;
    }
}