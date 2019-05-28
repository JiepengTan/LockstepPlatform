using Entitas;
using Lockstep.Math;

namespace Lockstep.ECS.Game {
  
    [Game]
    [System.Serializable]
    public partial class DropRateComponent : IComponent {
        public LFloat value;
    }
}