using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game.UI {
    public class ReferenceHolder : IReferenceHolder {
        private Dictionary<string, Object> _name2Objs = new Dictionary<string, Object>();

        public T GetRef<T>(string name) where T : Object{
            return _name2Objs.GetRefVal(name) as T;
        }

        public void AddRef(string name, Object obj){
            if (_name2Objs.ContainsKey(name)) {
                UnityEngine.Debug.LogError($" Add failed! {name} already exist!");
                return;
            }

            _name2Objs[name] = obj;
        }

        public void ReplaceRef(string name, Object obj){
            if (!_name2Objs.ContainsKey(name)) {
                UnityEngine.Debug.LogError($" Add failed! {name} do not exist!");
                return;
            }

            _name2Objs[name] = obj;
        }

        public bool DeleteRef(string name){
            return _name2Objs.Remove(name);
        }
    }
}