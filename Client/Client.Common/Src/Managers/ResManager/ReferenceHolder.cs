using UnityEngine;

namespace Lockstep.Game {

    public partial class ReferenceHolder :BaseSystemReferenceHolder{
        
        protected IGameMsgService _gameMsgService;
        protected ISimulation _simulationService;
        protected IUIService _uiService;
        
        protected IEventRegisterService _eventRegisterService;
        
        public override void InitReference(IServiceContainer serviceContainer){
            base.InitReference(serviceContainer);
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
            
            _gameMsgService = serviceContainer.GetService<IGameMsgService>();
            _simulationService = serviceContainer.GetService<ISimulation>();
            _uiService = serviceContainer.GetService<IUIService>();
        }
    }
    
    
    public partial class ManagerReferenceHolder:ReferenceHolder {
        
        public Transform transform { get; protected set; }
        public GameObject gameObject{ get; protected set; }
        public void AssignReference(Contexts contexts, IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            //_contexts = contexts;
            //_serviceContainer = serviceContainer;
            //InitReference(mgrContainer);
            InitReference(contexts);
            InitReference(serviceContainer);
        }
        
        
    }
}