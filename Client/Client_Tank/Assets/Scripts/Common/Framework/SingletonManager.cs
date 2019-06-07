#define USE_UNITY_MONO
using System;
using UnityEngine;

namespace Lockstep.Game {
    public partial class ReferenceHolder { }

    public partial class ManagerReferenceHolder
#if USE_UNITY_MONO
        : MonoBehaviour {}
#else 
    {
        public Transform transform;
        public GameObject gameObject;
    }
#endif

    public partial class BaseManager : ManagerReferenceHolder, IService {
        public virtual void DoAwake(IServiceContainer services){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(float deltaTime){ }
        public virtual void DoFixedUpdate(){ }
        public virtual void DoDestroy(){ }
    }

    public abstract class SingletonManager<T> : BaseManager, ITimeMachine where T : SingletonManager<T>, new() {
        protected static T _instance;


        public static T Instance {
            get {
                if (_instance == null) {
#if USE_UNITY_MONO
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
#else
                    _instance = new T();
#endif
                }

                return _instance;
            }
        }

        public void Awake(){
            if (_instance != null && _instance != (T) this) {
#if USE_UNITY_MONO
                GameObject.Destroy(_instance);
#endif
                return;
            }

            _instance = (T) this;
#if USE_UNITY_MONO
            _instance.transform.SetParent(Main.Instance.transform, false);
#endif
            Main.Instance.RegisterManager(this);
            cmdBuffer = new CommandBuffer<T>();
            cmdBuffer.Init(_instance, GetRollbackFunc());
#if USE_UNITY_MONO
            if (Application.isPlaying) {
                GameObject.DontDestroyOnLoad(_instance.gameObject);    
            }
#endif
        }

        protected ICommandBuffer<T> cmdBuffer;
        public int CurTick { get; set; }

        protected virtual Action<CommandBuffer<T>.CommandNode, CommandBuffer<T>.CommandNode, T> GetRollbackFunc(){
            return null;
        }

        public virtual void Backup(int tick){ }

        public void RollbackTo(int tick){
            cmdBuffer?.Jump(CurTick, tick);
        }

        public void Clean(int maxVerifiedTick){
            cmdBuffer?.Clean(maxVerifiedTick);
        }
    }
}