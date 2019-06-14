using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{
    [Game]
    public partial class OwnerComponent : IComponent {
        public uint localId;
    }
}