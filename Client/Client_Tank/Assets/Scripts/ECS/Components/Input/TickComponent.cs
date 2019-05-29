using Entitas;

namespace Lockstep.ECS.Input
{                 
    [Input]
    public partial class TickComponent : IComponent
    {
        public int value;
    }
}
