using UnityEngine;

namespace Lockstep.Game {

    public partial class ManagerReferenceHolder {
        #region Mgrs
        protected AudioManager _audioMgr;
        protected InputManager _inputMgr;
        protected MapManager MapMgr;
        protected GameManager _gameMgr;
        protected UIManager _uiMgr;
        protected SimulationManager _simulationMgr;
        protected NetworkManager _networkMgr;


        protected Contexts _contexts;
        protected IServiceContainer _serviceContainer;
        
        public void AssignReference(Contexts contexts, IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            _contexts = contexts;
            _serviceContainer = serviceContainer;
            InitReference(mgrContainer);
            InitReference(contexts);
            InitReference(serviceContainer);
        }

        public void InitReference(IManagerContainer mgrContainer){
            _audioMgr = mgrContainer.GetManager<AudioManager>();
            _inputMgr = mgrContainer.GetManager<InputManager>();
            MapMgr = mgrContainer.GetManager<MapManager>();
            _gameMgr = mgrContainer.GetManager<GameManager>();
            _uiMgr = mgrContainer.GetManager<UIManager>();
            _simulationMgr = mgrContainer.GetManager<SimulationManager>();
            _networkMgr = mgrContainer.GetManager<NetworkManager>();
        }
        
        #endregion
        
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;

        protected IResourceService _resourceService;
        protected IAudioService _audioService;
        protected IInputService _inputService;
        protected IMapService _mapService;
        protected IConfigService _configService;
        protected IEventRegisterService _eventRegisterService;
        

        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
            _configService = serviceContainer.GetService<IConfigService>();
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
        }

        public void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
    }
    
    public partial class ReferenceHolder {
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;

        protected IResourceService _resourceService;
        protected IAudioService _audioService;
        protected IInputService _inputService;
        protected IMapService _mapService;
        protected IConfigService _configService;
        protected IEventRegisterService _eventRegisterService;
        

        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
            _configService = serviceContainer.GetService<IConfigService>();
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
        }

        public void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
    }
}