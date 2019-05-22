using UnityEngine;

namespace Lockstep.Game {
    public partial class ReferenceHolder { }

    public partial class ManagerReferenceHolder : MonoBehaviour { }

    public partial class BaseManager : ManagerReferenceHolder, IService {
        public uint CurTick { get; set; }
        public Main main { get; private set; }
        public virtual void DoAwake(IServiceContainer services){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(float deltaTime){ }
        public virtual void DoFixedUpdate(){ }
        public virtual void DoDestroy(){ }
    }

    public abstract class SingletonManager<T> : BaseManager, IRollbackable where T : SingletonManager<T> {
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
            cmdBuffer = new CommandBuffer<T>(_instance);
            GameObject.DontDestroyOnLoad(_instance.gameObject);
        }

        protected CommandBuffer<T> cmdBuffer;
        public virtual void BackUp(uint tick){ }

        public void RevertTo(uint tick){
            cmdBuffer?.RevertTo(tick);
        }

        public void Clean(uint maxVerifiedTick){
            cmdBuffer?.Clean(maxVerifiedTick);
        }
    }
}