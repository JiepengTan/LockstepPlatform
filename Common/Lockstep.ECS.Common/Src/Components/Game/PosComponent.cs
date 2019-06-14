using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Game
{
    [Game, Event(EventTarget.Self)]
    public partial class PosComponent : IComponent
    {
        public LVector2 value;
    }
}