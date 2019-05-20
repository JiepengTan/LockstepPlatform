using UnityEngine;

namespace Lockstep.Game.Systems {
    public class BaseSystem {
        protected readonly IResourceService _resourceService;
        protected readonly IAudioService _audioService;
        protected readonly GameContext _gameContext;
        protected readonly GameStateContext _gameStateContext;
        protected readonly ActorContext _actorContext;
        
        
        public BaseSystem(Contexts contexts, IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
              
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
            _actorContext = contexts.actor;
        }
    }
}