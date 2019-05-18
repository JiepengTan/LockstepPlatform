using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState
{
    [GameState, Unique]
    public class TickComponent : IComponent
    {                         
        public uint value;
    }     
    
}