using Lockstep.Math;
using Entitas;

namespace Lockstep.ECS.Input
{
    [Input]
    public class CoordinateComponent : IComponent
    {                               
        public LVector2 value;
    }
}