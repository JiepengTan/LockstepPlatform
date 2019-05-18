using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Snapshot
{                 
    [Snapshot]
    public class TickComponent : IComponent
    {
        [PrimaryEntityIndex]
        public uint value;
    }
}
