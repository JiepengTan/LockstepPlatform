using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.Core.State.Game
{
    [Game, Event(true)]
    public class DestinationComponent : IComponent
    {
        public LVector2 value;
    }
}