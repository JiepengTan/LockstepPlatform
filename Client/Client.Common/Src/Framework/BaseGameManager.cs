//#define USE_UNITY_MONO

using System;
using UnityEngine;

namespace Lockstep.Game {
    public abstract class BaseGameManager : BaseManager {
        
        protected IServiceContainer serviceContainer;
        public void AssignReference(Contexts contexts, IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            this.serviceContainer = serviceContainer;
            InitReference(contexts);
            InitReference(serviceContainer);
            InitMgrReference(serviceContainer);
        }
        
        protected IEventRegisterService _eventRegisterService;
        protected IGameMsgService _gameMsgService;
        protected ISimulation _simulationService;
        protected IUIService _uiService;
        public void InitMgrReference(IServiceContainer serviceContainer){
            
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
            _gameMsgService = serviceContainer.GetService<IGameMsgService>();
            _simulationService = serviceContainer.GetService<ISimulation>();
            _uiService = serviceContainer.GetService<IUIService>();
        }
        
        
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


        public override void InitReference(IServiceContainer serviceContainer){
            base.InitReference(serviceContainer);
            //游戏相关的Survice
            _gameEffectService = serviceContainer.GetService<IGameEffectService>();
            _gameAudioService = serviceContainer.GetService<IGameAudioService>();
            _gameUnitService = serviceContainer.GetService<IGameUnitService>();
            _gameConstStateService = serviceContainer.GetService<IGameConstStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _gameCollisionService = serviceContainer.GetService<IGameCollisionService>();
            
        }
    }
}