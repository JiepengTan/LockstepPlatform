using Entitas;

namespace Lockstep.ECS.Actor
{
    [Actor]
    public partial class EntityCountComponent : IComponent
    {
        public uint value;
    }
}
