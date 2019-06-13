using UnityEngine;

namespace Lockstep.Game {
    public static class ExtensionUtils {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component{
            var comp = obj.GetComponent<T>();
            if (comp != null) return comp;
            return obj.AddComponent<T>();
        }
    }
}