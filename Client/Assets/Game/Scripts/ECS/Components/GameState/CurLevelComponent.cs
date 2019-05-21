using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState {
    [GameState]
    [Unique]
    public class CurLevelComponent : IComponent {
        public int value;
    }
}