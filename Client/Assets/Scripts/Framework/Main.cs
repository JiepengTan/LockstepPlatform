using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lockstep.Logging;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public enum EDir {
        Up,
        Left,
        Down,
        Right,
        EnumCount,
    }

    public partial class Main : ManagerReferenceHolder, IServiceContainer, IManagerContainer {
        public static Main Instance { get; private set; }
        public Contexts contexts;
        private Dictionary<string, BaseManager> name2Mgr = new Dictionary<string, BaseManager>();
        private List<BaseManager> allMgrs = new List<BaseManager>();
        private Dictionary<string, IService> allServices = new Dictionary<string, IService>();

        
        #region IManagerContainer
        public void RegisterManager(BaseManager manager){
            var name = manager.GetType().Name;
            if (name2Mgr.ContainsKey(name)) {
                Debug.LogError(
                    $"Duplicate register manager {name} type:{manager?.GetType().ToString() ?? ""} goName:{manager?.name ?? ""}");
                return;
            }

            name2Mgr.Add(name, manager);
            allMgrs.Add(manager);
            RegisterService(manager);
        }

        public T GetManager<T>() where T : BaseManager{
            if (name2Mgr.TryGetValue(typeof(T).Name, out var val)) {
                return val as T;
            }

            return null;
        }

        #endregion

        #region IServiceContainer

        public void RegisterService(IService service, bool overwriteExisting = true){
            var interfaceNames = service.GetType().FindInterfaces((type, criteria) =>
                    type.GetInterfaces()
                        .Any(t => t.FullName == typeof(IService).FullName), service)
                .Select(type => type.FullName).ToArray();

            foreach (var name in interfaceNames) {
                if (!allServices.ContainsKey(name))
                    allServices.Add(name, service);
                else if (overwriteExisting) {
                    allServices[name] = service;
                }
            }
        }

        public T GetService<T>() where T : IService{
            var key = typeof(T).FullName;
            if (key == null) {
                return default(T);
            }

            if (!allServices.ContainsKey(key)) {
                return default(T);
            }

            return (T) allServices[key];
        }
        

        #endregion

        #region LifeCycle

        

        private void Awake(){
            if (Instance != null) {
                Debug.LogError("Error: has 2 main scripts!!");
                GameObject.Destroy(this.gameObject);
            }

            Instance = this;
            contexts = new Contexts();
            Log.OnMessage += OnLog;
            DoAwake();
        }


        private void Start(){
            this.AssignReference(this);
            foreach (var mgr in allMgrs) {
                mgr.AssignReference(this);
            }
            foreach (var mgr in allMgrs) {
                mgr.DoAwake();
            }
            DoStart();
            foreach (var mgr in allMgrs) {
                mgr.DoStart();
            }
        }

        void Update(){
            var deltaTime = Time.deltaTime;
            DoUpdate(deltaTime);
            foreach (var mgr in allMgrs) {
                mgr.DoUpdate(deltaTime);
            }
        }

        private void FixedUpdate(){
            DoFixedUpdate();
            foreach (var mgr in allMgrs) {
                mgr.DoFixedUpdate();
            }
        }


        private void OnDestroy(){
            foreach (var mgr in allMgrs) {
                mgr.DoDestroy();
            }
            DoDestroy();
        }

        #endregion
        void OnLog(object sender, LogEventArgs args){
            switch (args.LogSeverity) {
                case LogSeverity.Info:
                    UnityEngine.Debug.Log(args.Message);
                    break;
                case LogSeverity.Warn:
                    UnityEngine.Debug.LogWarning(args.Message);
                    break;
                case LogSeverity.Error:
                    UnityEngine.Debug.LogError(args.Message);
                    break;
                case LogSeverity.Exception:
                    UnityEngine.Debug.LogError(args.Message);
                    break;
            }
        }
    }
}