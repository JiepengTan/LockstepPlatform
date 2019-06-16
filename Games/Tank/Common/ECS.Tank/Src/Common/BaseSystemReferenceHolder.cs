namespace Lockstep.Game {
    public class BaseSystemReferenceHolder :ServiceReferenceHolder{
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;
        
        
        public virtual void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
        
        
        protected IGameConstStateService _gameConstStateService;
        protected IGameStateService _gameStateService;
        protected IGameEffectService _gameEffectService;
        protected IGameAudioService _gameAudioService;
        protected IGameUnitService _gameUnitService;
        protected IGameCollisionService _gameCollisionService;
        protected IGameConfigService _gameConfigService;
        

        public override void InitReference(IServiceContainer serviceContainer){
            base.InitReference(serviceContainer);
            _gameEffectService = serviceContainer.GetService<IGameEffectService>();
            _gameAudioService = serviceContainer.GetService<IGameAudioService>();
            _gameUnitService = serviceContainer.GetService<IGameUnitService>();
            _gameConstStateService = serviceContainer.GetService<IGameConstStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _gameCollisionService = serviceContainer.GetService<IGameCollisionService>();
            _gameConfigService = serviceContainer.GetService<IGameConfigService>();
            
        }
    }
}