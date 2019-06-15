using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public class ManagerContainer : IManagerContainer {
        private Dictionary<string, BaseManager> _name2Mgr = new Dictionary<string, BaseManager>();
        public List<BaseManager> AllMgrs = new List<BaseManager>();

        public void RegisterManager(BaseManager manager){
            var name = manager.GetType().Name;
            if (_name2Mgr.ContainsKey(name)) {
                Debug.LogError(
                    $"Duplicate Register manager {name} type:{manager?.GetType().ToString() ?? ""} goName:{manager?.gameObject.name ?? ""}");
                return;
            }

            _name2Mgr.Add(name, manager);
            AllMgrs.Add(manager);
        }

        public T GetManager<T>() where T : BaseManager{
            if (_name2Mgr.TryGetValue(typeof(T).Name, out var val)) {
                return val as T;
            }

            return null;
        }

        public void Foreach(Action<BaseManager> func){
            foreach (var mgr in AllMgrs) {
                func(mgr);
            }
        }
    }
}