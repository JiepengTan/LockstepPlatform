using UnityEngine;
namespace Lockstep.Game
{
    public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance();
                }
                return _instance;
            }
        }
        private static T CreateInstance()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                Object.DontDestroyOnLoad(go);
                T t = go.AddComponent<T>();
                _instance = t;
            }
            return _instance;
        }
    }
}
