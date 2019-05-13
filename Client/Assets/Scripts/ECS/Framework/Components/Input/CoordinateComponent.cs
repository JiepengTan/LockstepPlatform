using Lockstep.Math;
using Entitas;

namespace Lockstep.Core.State.Input
{
    [Input]
    public class CoordinateComponent : IComponent
    {                               
        public LVector2 value;
    }
}