namespace Lockstep.Game {
    public partial class ServiceReferenceHolder {
        protected IRandomService _randomService;
        protected ITimeMachineService _timeMachineService;
        protected IConstStateService _constStateService;
        protected IViewService _viewService;
        protected IAudioService _audioService;
        protected IInputService _inputService;
        protected IMap2DService _map2DService;
        protected IResService _resService;
        protected IEffectService _effectService;
        
        
        protected IGameConstStateService _gameConstStateService;
        protected IGameStateService _gameStateService;
        protected IGameEffectService _gameEffectService;
        protected IGameAudioService _gameAudioService;
        protected IGameUnitService _gameUnitService;
        
        public virtual void InitReference(IServiceContainer serviceContainer){
            //通用Service
            _randomService = serviceContainer.GetService<IRandomService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            _constStateService = serviceContainer.GetService<IConstStateService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _viewService = serviceContainer.GetService<IViewService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _map2DService = serviceContainer.GetService<IMap2DService>();
            _resService = serviceContainer.GetService<IResService>();
            _effectService = serviceContainer.GetService<IEffectService>();
            
            //游戏相关的Survice
            _gameEffectService = serviceContainer.GetService<IGameEffectService>();
            _gameAudioService = serviceContainer.GetService<IGameAudioService>();
            _gameUnitService = serviceContainer.GetService<IGameUnitService>();
            _gameConstStateService = serviceContainer.GetService<IGameConstStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
        }
    }
   
}