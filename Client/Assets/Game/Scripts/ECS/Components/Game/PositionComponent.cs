using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Game
{
    [Game, Event(true)]
    public class PositionComponent : IComponent
    {
        public LVector2 value;
    }
}