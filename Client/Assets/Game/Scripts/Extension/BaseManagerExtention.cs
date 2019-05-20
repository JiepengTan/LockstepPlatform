using UnityEngine;

namespace Lockstep.Game {


    
    
    public partial class ManagerReferenceHolder {
        #region Mgrs
        protected AudioManager _audioMgr;
        protected InputManager _inputMgr;
        protected LevelManager _levelMgr;
        protected GameManager _gameMgr;
        protected UIManager _uiMgr;
        protected SimulationManager _simulationMgr;
        protected NetworkManager _networkMgr;

        public void AssignReference(Contexts contexts, IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            InitReference(mgrContainer);
            InitReference(contexts);
            InitReference(serviceContainer);
        }

        public void InitReference(IManagerContainer mgrContainer){
            _audioMgr = mgrContainer.GetManager<AudioManager>();
            _inputMgr = mgrContainer.GetManager<InputManager>();
            _levelMgr = mgrContainer.GetManager<LevelManager>();
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
        

        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
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
        

        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
        }

        public void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
    }
}