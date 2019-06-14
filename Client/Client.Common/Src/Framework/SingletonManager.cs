//#define USE_UNITY_MONO

using System;
using UnityEngine;

namespace Lockstep.Game {


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
                    _instance = new T();
                }

                return _instance;
            }
        }

        public void Awake(Transform parent){
            if (_instance != null && _instance != (T) this) {
                return;
            }

            var go = new GameObject(GetType().Name);
            gameObject = go;
            transform = go.transform;

            _instance = (T) this;
            _instance.transform.SetParent(parent, false);
            cmdBuffer = new CommandBuffer<T>();
            cmdBuffer.Init(_instance, GetRollbackFunc());
        }

        public override void DoDestroy(){
            if (gameObject != null) {
                if (Application.isPlaying) {
                    GameObject.Destroy(gameObject);
                }
                else {
                    GameObject.DestroyImmediate(gameObject);
                }
            }
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