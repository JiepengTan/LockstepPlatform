using UnityEngine;

namespace Lockstep.Game {
    public partial class ManagerReferenceHolder {
        #region Mgrs

 

        #endregion
        public void AssignReference(Contexts contexts, IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            //_contexts = contexts;
            //_serviceContainer = serviceContainer;
            //InitReference(mgrContainer);
            InitReference(contexts);
            InitReference(serviceContainer);
        }
        
        
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;

        protected IResourceService _resourceService;
        protected IAudioService _audioService;
        protected IInputService _inputService;
        protected IMapService _mapService;
        protected IEventRegisterService _eventRegisterService;
        protected IViewService _viewService;
        protected IUnitService _unitService;
        protected IRandomService _randomService;
        protected ITimeMachineService _timeMachineService;
        protected IConstGameStateService _constStateService;
        protected IGameStateService _gameStateService;
        protected INetworkService _networkService;
        
        
        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
            _viewService = serviceContainer.GetService<IViewService>();
            _unitService = serviceContainer.GetService<IUnitService>();
            _randomService = serviceContainer.GetService<IRandomService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            _constStateService = serviceContainer.GetService<IConstGameStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _networkService = serviceContainer.GetService<INetworkService>();
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
        protected IEventRegisterService _eventRegisterService;
        protected IViewService _viewService;
        protected IUnitService _unitService;
        protected IRandomService _randomService;
        protected ITimeMachineService _timeMachineService;
        protected IConstGameStateService _constStateService;
        protected IGameStateService _gameStateService;
        protected INetworkService _networkService;
        
        
        public void InitReference(IServiceContainer serviceContainer){
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _mapService = serviceContainer.GetService<IMapService>();
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
            _viewService = serviceContainer.GetService<IViewService>();
            _unitService = serviceContainer.GetService<IUnitService>();
            _randomService = serviceContainer.GetService<IRandomService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            _constStateService = serviceContainer.GetService<IConstGameStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _networkService = serviceContainer.GetService<INetworkService>();
        }

        public void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
    }
}