using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class OwnerComponent : IComponent {
        public uint localId;
    }
}