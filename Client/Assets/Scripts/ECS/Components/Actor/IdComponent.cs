using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Actor
{
    [Actor] 
    public sealed class IdComponent : IComponent
    {
        [PrimaryEntityIndex]
        public byte value;
    }
}