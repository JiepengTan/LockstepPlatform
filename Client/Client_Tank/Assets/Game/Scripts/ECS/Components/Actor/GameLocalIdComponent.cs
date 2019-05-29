using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Actor
{

    [Actor]
    public partial class GameLocalIdComponent : IComponent {
        public uint value;
    }
}