using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
  
    [Game]
    public partial class DropRateComponent : IComponent {
        public LFloat value;
    }
}