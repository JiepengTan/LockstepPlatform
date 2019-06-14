namespace Lockstep.Game {
    public partial class ServiceReferenceHolder {
        protected IRandomService _randomService;
        protected ITimeMachineService _timeMachineService;
        
        protected IResourceService _resourceService;
        protected IGameAudioService _audioService;
        protected IUnitService _unitService;
        protected IConstGameStateService _constGameStateService;
        protected IGameStateService _gameStateService;
        protected IMapService _mapService;

        public virtual void InitReference(IServiceContainer serviceContainer){
            _randomService = serviceContainer.GetService<IRandomService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            
            _resourceService = serviceContainer.GetService<IResourceService>();
            _audioService = serviceContainer.GetService<IGameAudioService>();
            _unitService = serviceContainer.GetService<IUnitService>();
            _constGameStateService = serviceContainer.GetService<IConstGameStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _mapService = serviceContainer.GetService<IMapService>();
        }
    }
   
}