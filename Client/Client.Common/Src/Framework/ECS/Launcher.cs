using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Lockstep.Core;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    [Serializable]
    public class Launcher : ILifeCycle {
        private int _curTick;

        public int CurTick {
            get => _curTick;
            set {
                _curTick = value;
                _timeMachineContainer.CurTick = value;
            }
        }

        public static Launcher Instance { get; private set; }
        public IContexts Contexts;

        private ServiceContainer _serviceContainer;
        private ManagerContainer _mgrContainer;
        private TimeMachineContainer _timeMachineContainer;
        private IEventRegisterService _registerService;
        public MainManager _mainManager = new MainManager();

        [ShowNativeProperty]
        public MainManager MainManager {
            get => _mainManager;
            set => _mainManager = value;
        }

        public void DoAwake(IServiceContainer services){
            if (Instance != null) {
                Debug.LogError("LifeCycle Error: Awake more than once!!");
                return;
            }

            Instance = this;
            _serviceContainer = new ServiceContainer();
            _mgrContainer = new ManagerContainer();
            _timeMachineContainer = new TimeMachineContainer();
            _registerService = new EventRegisterService();

            Lockstep.Logging.Logger.OnMessage += UnityLogHandler.OnLog;
            //AutoCreateManagers;
            var types = typeof(Launcher).Assembly.GetTypes();
            foreach (var ty in types) {
                if (ty.IsSubclassOf(typeof(BaseService)) && !ty.IsAbstract) {
                    if (ty.GetInterfaces().Any(t => t == typeof(ITimeMachine))) {
                        var property = ty.GetProperty("Instance",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                        if (property != null) {
                            var inst = property.GetValue(null, null);
                            //call Awake method
                            if (inst != null) {
                                var method = ty.GetMethod("Awake",
                                    BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                                if (method != null) {
                                    method.Invoke(inst, new object[] { });
                                }

                                if (inst is BaseService manager) {
                                    _mgrContainer.RegisterManager(manager);
                                    _serviceContainer.RegisterService(manager);
                                    _timeMachineContainer.RegisterTimeMachine(manager as ITimeMachine);
                                }
                            }
                        }
                    }
                }
            }

            Contexts = _serviceContainer.GetService<IECSFacadeService>().CreateContexts();
            _serviceContainer.GetService<IConstStateService>().Contexts = Contexts;
            _serviceContainer.RegisterService(_timeMachineContainer);
            _serviceContainer.RegisterService(_registerService);
        }


        public void DoStart(){
            foreach (var mgr in _mgrContainer.AllMgrs) {
                if (mgr is IBaseGameManager gameMgr) {
                    gameMgr.AssignReference(_serviceContainer, _mgrContainer);
                }
                else {
                    mgr.InitReference(_serviceContainer);
                }
            }

            //bind events
            foreach (var mgr in _mgrContainer.AllMgrs) {
                _registerService.RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
                    EventHelper.AddListener, mgr);
            }

            _mainManager.DoAwake(_serviceContainer);
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoAwake(_serviceContainer);
            }

            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoStart();
            }

            _mainManager.DoStart();
        }

        public void DoUpdate(int deltaTimeMs){
            _mainManager.DoUpdate(deltaTimeMs);
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoUpdate(deltaTimeMs);
            }
        }

        public void DoFixedUpdate(){
            _mainManager.DoFixedUpdate();
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoFixedUpdate();
            }
        }


        public void DoDestroy(){
            if (Instance == null) return;
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoDestroy();
            }

            _mainManager.DoDestroy();
            Instance = null;
        }

        public void OnApplicationQuit(){
            DoDestroy();
        }
    }
}