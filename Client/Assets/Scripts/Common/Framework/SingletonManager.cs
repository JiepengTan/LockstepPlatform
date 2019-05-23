using System;
using UnityEngine;

namespace Lockstep.Game {
    public partial class ReferenceHolder { }

    public partial class ManagerReferenceHolder : MonoBehaviour { }

    public partial class BaseManager : ManagerReferenceHolder, IService {
        public Main main { get; private set; }
        public virtual void DoAwake(IServiceContainer services){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(float deltaTime){ }
        public virtual void DoFixedUpdate(){ }
        public virtual void DoDestroy(){ }
    }

    public abstract class SingletonManager<T> : BaseManager, ITimeMachine where T : SingletonManager<T> {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }

                return _instance;
            }
        }

        protected void Awake(){
            if (_instance != null) {
                GameObject.Destroy(this);
                return;
            }

            _instance = (T) this;
            _instance.transform.SetParent(Main.Instance.transform,false);
            Main.Instance.RegisterManager(this);
            cmdBuffer = new CommandBuffer<T>(_instance,GetRollbackFunc());
            GameObject.DontDestroyOnLoad(_instance.gameObject);
        }

        protected CommandBuffer<T> cmdBuffer;
        public int CurTick { get; set; }

        protected virtual Action<CommandBuffer<T>.CommandNode, CommandBuffer<T>.CommandNode, T> GetRollbackFunc(){return null;}
        public virtual void Backup(int tick){ }

        public void RollbackTo(int tick){
            cmdBuffer?.RevertTo(tick);
        }
        public void Clean(int maxVerifiedTick){
            cmdBuffer?.Clean(maxVerifiedTick);
        }
    }
}