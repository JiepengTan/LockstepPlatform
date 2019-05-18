using Lockstep.Math;
using Entitas;

namespace Lockstep.Core.State.Game
{
    [Game]
    public class VelocityComponent : IComponent
    {
        public LVector2 value;
    }
}