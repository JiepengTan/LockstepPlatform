using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{

    [Actor,Event(EventTarget.Self)]
    public partial class ScoreComponent : IComponent {
        public int value;
    }
}