using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lockstep.Game {
    public static class ExtensionGameObject {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component{
            var comp = obj.GetComponent<T>();
            if (comp != null) return comp;
            return obj.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject obj, Type type) where T : Component{
            var comp = obj.GetComponent(type) as T;
            if (comp != null) return comp;
            return obj.AddComponent(type) as T;
        }

        public static string HierarchyName(this Component comp){
            return comp.transform.HierarchyName();
        }
        public static string HierarchyName(this GameObject go){
            return go.transform.HierarchyName();
        }
        public static string HierarchyName(this Transform trans){
            Stack<string> transNames = new Stack<string>();
            StringBuilder sb = new StringBuilder();
            while (trans != null) {
                transNames.Push(trans.name);
                trans = trans.parent;
            }

            while (transNames.Count > 0) {
                sb.Append(transNames.Pop());
                if (transNames.Count > 0) {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }
    }
}