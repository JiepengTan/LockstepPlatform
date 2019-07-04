using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Lockstep.Util;
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
        public string GameName = "";
        public bool IsDebugMode = false;
        public IContexts Contexts;

        private ServiceContainer _serviceContainer;
        private ManagerContainer _mgrContainer;
        private TimeMachineContainer _timeMachineContainer;
        private IEventRegisterService _registerService;
        public MainManager _mainManager = new MainManager();

        public EPureModeType RunMode = EPureModeType.Unity; //纯净模式  不含Unity 相关的代码
        public object transform;

        [ShowNativeProperty]
        public MainManager MainManager {
            get => _mainManager;
            set => _mainManager = value;
        }

        public T GetService<T>() where T : IService{
            return _serviceContainer.GetService<T>();
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type[] types = null;
            {
                types = assemblies.SelectMany((Assembly assembly) => assembly.GetTypes())
                    .Where((Type tt) => tt.GetInterfaces().Contains(typeof(IService)) && !tt.IsAbstract).ToArray();
            }
            {
                var pureMode = RunMode;
                var typeLst = types.Where((Type tt) => {
                        var attris = tt.GetCustomAttributes(typeof(PureModeAttribute), true);
                        if (attris.Length > 0) {
                            var attri = attris[0] as PureModeAttribute;
                            if (attri.Type != pureMode) {
                                return false;
                            }
                        }

                        return true;
                    }
                ).ToList();
                typeLst.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
                types = typeLst.ToArray();
            }

            //var types = ReflectionUtility.GetTypes();
            foreach (var ty in types) {
                var service = (IService) Activator.CreateInstance(ty);
                var method = ty.GetMethod("DoInit",
                    BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                if (method != null) {
                    try {
                        method.Invoke(service, new object[] {transform});
                    }
                    catch (Exception e) {
                        Debug.LogError("" + service.GetType() + "  " + e);
                        throw;
                    }
                }

                _serviceContainer.RegisterService(service);
                _timeMachineContainer.RegisterTimeMachine(service as ITimeMachine);
                if (service is BaseService baseService) {
                    _mgrContainer.RegisterManager(baseService);
                }
            }

            Contexts = _serviceContainer.GetService<IECSFacadeService>().CreateContexts();
            _serviceContainer.GetService<IConstStateService>().Contexts = Contexts;
            _serviceContainer.GetService<IConstStateService>().RunMode = RunMode;
            _serviceContainer.GetService<IConstStateService>().GameName = GameName;
            _serviceContainer.GetService<IConstStateService>().IsDebugMode = IsDebugMode;

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