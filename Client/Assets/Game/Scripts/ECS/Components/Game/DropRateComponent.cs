using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
  
    [Game]
    [System.Serializable]
    public class DropRateComponent : IComponent {
        public LFloat value;
    }
}