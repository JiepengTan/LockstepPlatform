using Entitas;

namespace Lockstep.ECS.Actor
{
    [Actor]
    public class EntityCountComponent : IComponent
    {
        public uint value;
    }
}
