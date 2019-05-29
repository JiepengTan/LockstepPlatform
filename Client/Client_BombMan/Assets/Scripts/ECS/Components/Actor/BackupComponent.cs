using Entitas;

namespace Lockstep.ECS.Actor
{
    [Actor]
    //An ActorEntity with BackupComponent refers to an actor in the past
    public partial class BackupComponent : IComponent
    {
        public byte actorId;

        public int tick;
    }
}
