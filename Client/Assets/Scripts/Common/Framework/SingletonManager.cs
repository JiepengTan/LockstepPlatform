using UnityEngine;

namespace Lockstep.Game {
    public partial class ReferenceHolder { }

    public partial class ManagerReferenceHolder : MonoBehaviour { }

    public partial class BaseManager : ManagerReferenceHolder, IRollbackable, IService {
        public Main main { get; private set; }
        public virtual void DoAwake(IServiceContainer services){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(float deltaTime){ }
        public virtual void DoFixedUpdate(){ }
        public virtual void DoDestroy(){ }

        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        public virtual void RevertTo(uint tick){ }

        public virtual void BackUp(uint tick){ }

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        public virtual void Clean(uint maxVerifiedTick){ }
    }

    public partial class SingletonManager<T> : BaseManager where T : SingletonManager<T> {
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
            Main.Instance.RegisterManager(this);
            GameObject.DontDestroyOnLoad(_instance.gameObject);
        }
    }
}