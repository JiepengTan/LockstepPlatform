using System;
using UnityEngine;

namespace Lockstep.Game {
    public partial class BaseManager : ServiceReferenceHolder, IService, ILifeCycle, ITimeMachine {
        public virtual void DoAwake(IServiceContainer services){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(int deltaTimeMs){ }
        public virtual void DoFixedUpdate(){ }
        public virtual void OnApplicationQuit(){ }

        public Transform transform { get; protected set; }
        public GameObject gameObject { get; protected set; }

        public void DoAwake(Transform parent){
            cmdBuffer = new CommandBuffer();
            cmdBuffer.Init(this, GetRollbackFunc());
            var go = new GameObject(GetType().Name);
            gameObject = go;
            transform = go.transform;
            transform.SetParent(parent, false);
        }

        public virtual void DoDestroy(){
            if (gameObject != null) {
                if (Application.isPlaying) {
                    GameObject.Destroy(gameObject);
                }
                else {
                    GameObject.DestroyImmediate(gameObject);
                }
            }
        }

        protected ICommandBuffer cmdBuffer;

        protected virtual FuncUndoCommands GetRollbackFunc(){
            return null;
        }

        public int CurTick { get; set; }

        public virtual void Backup(int tick){ }

        public virtual void RollbackTo(int tick){
            cmdBuffer?.Jump(CurTick, tick);
        }

        public virtual void Clean(int maxVerifiedTick){
            cmdBuffer?.Clean(maxVerifiedTick);
        }
    }
}