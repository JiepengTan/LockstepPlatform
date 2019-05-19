using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Input
{
    [Input]
    public class MoveDirComponent : IComponent
    {                               
        public EDir value;
    }
}