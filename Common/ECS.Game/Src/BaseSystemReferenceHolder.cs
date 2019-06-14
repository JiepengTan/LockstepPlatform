namespace Lockstep.Game {

    public partial class BaseSystemReferenceHolder {
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;
        
        protected IRandomService _randomService;
        protected IResourceService _resourceService;
        protected IAudioService _audioService;
        protected IUnitService _unitService;
        protected ITimeMachineService _timeMachineService;
        protected IConstGameStateService _constStateService;
        protected IGameStateService _gameStateService;
        protected IMapService _mapService;
        
        public virtual void InitReference(IServiceContainer serviceContainer){
            _randomService = serviceContainer.GetService<IRandomService>();
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _unitService = serviceContainer.GetService<IUnitService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            _constStateService = serviceContainer.GetService<IConstGameStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _mapService = serviceContainer.GetService<IMapService>();
        }

        public virtual void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
        
    }
}