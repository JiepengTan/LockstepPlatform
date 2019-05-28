using Entitas;

namespace Lockstep.ECS.Input
{
    [Input]                                        
    public partial class ActorIdComponent : IComponent
    {
        public byte value;
    }
}