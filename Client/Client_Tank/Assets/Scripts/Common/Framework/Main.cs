using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lockstep.Core;
using UnityEngine;
using Lockstep.Logging;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public interface IEventRegisterService : IService {
        void RegisterManagersEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack) where TDelegate : Delegate
            where TEnum : struct;

        void RegisterEvent(object obj);
        void UnRegisterEvent(object obj);

        void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack, object obj)
            where TDelegate : Delegate
            where TEnum : struct;
    }

    public interface ITimeMachineService : ITimeMachine, IService {
        void RegisterTimeMachine(ITimeMachine roll);
    }

    public class MainScript : MonoBehaviour {
        public Main main;
    }

    public partial class Main : ManagerReferenceHolder, IServiceContainer, IManagerContainer, ITimeMachineService,
        IEventRegisterService {
        private int _curTick;

        public int CurTick {
            get { return _curTick; }
            set {
                _curTick = value;
                foreach (var timeMachine in GetAllTimeMachines()) {
                    timeMachine.CurTick = _curTick;
                }
            }
        }

        public static Main Instance { get; private set; }
        public Contexts contexts;

        #region LifeCycle

        public void Awake(){
            if (Instance != null) {
                Debug.LogError("Error: has 2 main scripts!!");
                GameObject.Destroy(this.gameObject);
            }

            Instance = this;
            contexts = Contexts.sharedInstance;
            Log.OnMessage += OnLog;
            //AutoCreateManagers;
            var types = typeof(Main).Assembly.GetTypes();
            foreach (var ty in types) {
                if (ty.IsSubclassOf(typeof(BaseManager)) && !ty.IsAbstract) {
                    if (ty.GetInterfaces().Any(t => t == typeof(ITimeMachine))) {
                        var property = ty.GetProperty("Instance",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                        if (property != null) {
                            var inst = property.GetValue(null, null);
#if UNITY_EDITOR
                            //call Awake method
                            if (!Application.isPlaying) {
                                if (inst != null) {
                                    var method = ty.GetMethod("Awake",
                                        BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                                    if (method != null) {
                                        method.Invoke(inst, new object[] { });
                                    }
                                }
                            }
#endif
                        }
                    }
                }
            }

            RegisterService(this);
        }


        public void Start(){
            this.AssignReference(contexts, this, this);
            foreach (var mgr in _allMgrs) {
                mgr.AssignReference(contexts, this, this);
            }

            //bind events
            RegisterManagersEvent();
            DoAwake();
            foreach (var mgr in _allMgrs) {
                mgr.DoAwake(this);
            }

            DoStart();
            foreach (var mgr in _allMgrs) {
                mgr.DoStart();
            }

            AfterStart();
        }

        public void Update(){
            _Update(Time.deltaTime);
        }

        void _Update(float deltaTime){
            DoUpdate(deltaTime);
            foreach (var mgr in _allMgrs) {
                mgr.DoUpdate(deltaTime);
            }
        }

        public void FixedUpdate(){
            DoFixedUpdate();
            foreach (var mgr in _allMgrs) {
                mgr.DoFixedUpdate();
            }
        }


        public void OnDestroy(){
            foreach (var mgr in _allMgrs) {
                mgr.DoDestroy();
            }

            DoDestroy();
            Instance = null;
        }

        #endregion

        #region ITimeMachineContainer

        private HashSet<ITimeMachine> _timeMachineHash = new HashSet<ITimeMachine>();
        private ITimeMachine[] _allTimeMachines;

        private ITimeMachine[] GetAllTimeMachines(){
            if (_allTimeMachines == null) {
                _allTimeMachines = _timeMachineHash.ToArray();
            }

            return _allTimeMachines;
        }

        public void RegisterTimeMachine(ITimeMachine roll){
            if (roll != null && _timeMachineHash.Add(roll)) ;
            {
                _allTimeMachines = null;
            }
        }

        public void RollbackTo(int tick){
            foreach (var timeMachine in GetAllTimeMachines()) {
                timeMachine.RollbackTo(tick);
            }
        }

        public void Backup(int tick){
            foreach (var timeMachine in GetAllTimeMachines()) {
                timeMachine.Backup(tick);
            }
        }

        public void Clean(int maxVerifiedTick){
            foreach (var timeMachine in GetAllTimeMachines()) {
                timeMachine.Clean(maxVerifiedTick);
            }
        }

        #endregion

        #region IManagerContainer

        private Dictionary<string, BaseManager> _name2Mgr = new Dictionary<string, BaseManager>();
        private List<BaseManager> _allMgrs = new List<BaseManager>();

        public void RegisterManager(BaseManager manager){
            var name = manager.GetType().Name;
            if (_name2Mgr.ContainsKey(name)) {
                Debug.LogError(
                    $"Duplicate Register manager {name} type:{manager?.GetType().ToString() ?? ""} goName:{manager?.gameObject.name ?? ""}");
                return;
            }

            _name2Mgr.Add(name, manager);
            _allMgrs.Add(manager);
            RegisterService(manager);
            RegisterTimeMachine(manager as ITimeMachine);
        }

        public T GetManager<T>() where T : BaseManager{
            if (_name2Mgr.TryGetValue(typeof(T).Name, out var val)) {
                return val as T;
            }

            return null;
        }

        #endregion

        #region IServiceContainer

        private Dictionary<string, IService> _allServices = new Dictionary<string, IService>();

        public void RegisterService(IService service, bool overwriteExisting = true){
            var interfaceNames = service.GetType().FindInterfaces((type, criteria) =>
                    type.GetInterfaces()
                        .Any(t => t.FullName == typeof(IService).FullName), service)
                .Select(type => type.FullName).ToArray();

            foreach (var name in interfaceNames) {
                if (!_allServices.ContainsKey(name))
                    _allServices.Add(name, service);
                else if (overwriteExisting) {
                    _allServices[name] = service;
                }
            }
        }


        public T GetService<T>() where T : IService{
            var key = typeof(T).FullName;
            if (key == null) {
                return default(T);
            }

            if (!_allServices.ContainsKey(key)) {
                return default(T);
            }

            return (T) _allServices[key];
        }

        #endregion

        #region IEventRegisterService

        public static T CreateDelegateFromMethodInfo<T>(System.Object instance, MethodInfo method) where T : Delegate{
            return Delegate.CreateDelegate(typeof(T), instance, method) as T;
        }

        public void UnRegisterEvent(object obj){
            RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
                EventHelper.RemoveListener, obj);
        }

        public void RegisterEvent(object obj){
            RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length, EventHelper.AddListener,
                obj);
        }

        private void RegisterManagersEvent(){
            RegisterManagersEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length, EventHelper.AddListener);
        }

        public void RegisterManagersEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack)
            where TDelegate : Delegate
            where TEnum : struct{
            if (callBack == null) return;
            foreach (var mgr in _allMgrs) {
                RegisterEvent(prefix, ignorePrefixLen, callBack, mgr);
            }
        }

        public void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack, object obj)
            where TDelegate : Delegate
            where TEnum : struct{
            if (callBack == null) return;
            var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                var methodName = method.Name;
                if (methodName.StartsWith(prefix)) {
                    var eventTypeStr = methodName.Substring(ignorePrefixLen);
                    if (Enum.TryParse(eventTypeStr, out TEnum eType)) {
                        var handler = CreateDelegateFromMethodInfo<TDelegate>(obj, method);
                        callBack(eType, handler);
                    }
                }
            }
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