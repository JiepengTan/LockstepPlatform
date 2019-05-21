using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.GameState
{        

    [GameState]
    [Unique]
    [Event(false, EventType.Added)]
    [Event(false, EventType.Removed)]
    public class GameResultComponent : IComponent {
        public EGameResult value;
    }
}