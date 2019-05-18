using System.Collections.Generic;
using Entitas;
using Lockstep.Game.Interfaces;

namespace Lockstep.Game.Features.Cleanup
{
    public class RemoveDestroyedEntitiesFromView : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly List<GameEntity> _buffer = new List<GameEntity>();

        private readonly IResourceService _resourceService;              

        public RemoveDestroyedEntitiesFromView(Contexts contexts, IServiceContainer services)
        {
            _group = contexts.game.GetGroup(GameMatcher.Destroyed);

            _resourceService = services.GetService<IResourceService>();               
        }

        public void Cleanup()
        {
            foreach (var e in _group.GetEntities(_buffer))
            {
                _resourceService.DeleteView(e.localId.value);        
            }
        }
    }
}