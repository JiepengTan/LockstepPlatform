using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
  
    [Game]
    public class DropRateComponent : IComponent {
        public LFloat value;
    }
}