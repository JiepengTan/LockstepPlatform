using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Actor
{

    [Actor]
    public partial class GameLocalIdComponent : IComponent {
        public uint value;
    }
}