using Lockstep.Math;
using Entitas;

namespace Lockstep.ECS.Game
{
    [Game]
    public partial class BornPointComponent : IComponent {
        public LVector2 coord;
    }
}