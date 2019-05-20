using Lockstep.Game.Systems.Cleanup;

namespace Lockstep.Game.Features
{
    sealed class CleanupFeature : Feature
    {
        public CleanupFeature(Contexts contexts, IServiceContainer services) : base("Cleanup")
        {
            Add(new SystemRemoveDestroyedEntitiesFromView(contexts, services));
        }
    }
}
