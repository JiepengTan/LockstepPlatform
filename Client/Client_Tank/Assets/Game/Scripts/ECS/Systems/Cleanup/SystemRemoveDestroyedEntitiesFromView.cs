using System.Collections.Generic;
using Entitas;

namespace Lockstep.Game.Systems.Cleanup
{
    public class SystemRemoveDestroyedEntitiesFromView : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly List<GameEntity> _buffer = new List<GameEntity>();

        private readonly IViewService _viewService;              

        public SystemRemoveDestroyedEntitiesFromView(Contexts contexts, IServiceContainer services)
        {
            _group = contexts.game.GetGroup(GameMatcher.Destroyed);
            _viewService = services.GetService<IViewService>();               
        }

        public void Cleanup()
        {
            foreach (var e in _group.GetEntities(_buffer))
            {
                _viewService.DeleteView(e.localId.value);        
            }
        }
    }
}