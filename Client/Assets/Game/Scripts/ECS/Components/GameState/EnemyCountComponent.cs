using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState
{        
    [GameState]
    [Unique]
    [Event(false, EventType.Added)]
    public class EnemyCountComponent : IComponent {
        public int value;
    }
}