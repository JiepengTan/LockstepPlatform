using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class ColliderComponent : IComponent {
        public LVector2 size;
        public LFloat radius;
    }
}