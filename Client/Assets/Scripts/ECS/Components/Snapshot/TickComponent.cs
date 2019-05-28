using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Snapshot
{                 
    [Snapshot]
    public partial class TickComponent : IComponent
    {
        [PrimaryEntityIndex]
        public int value;
    }
}
