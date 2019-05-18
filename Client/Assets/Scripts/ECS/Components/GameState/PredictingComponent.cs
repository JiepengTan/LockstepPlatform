using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState
{
    [GameState, Unique]
    public class PredictingComponent : IComponent
    {
    }
}
