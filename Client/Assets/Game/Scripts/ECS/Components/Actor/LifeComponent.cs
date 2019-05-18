using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Actor,Event(true)]
    public class LifeComponent : IComponent {
        public int value;
    }
}