using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PosListener : MonoBehaviour, IEventListener, IPosListener {
        private GameEntity _entity;
        public void RegisterListeners(IEntity entity){
            RegisterListeners(entity as GameEntity);
        }
        public void RegisterListeners(GameEntity entity){
            _entity = entity;
            _entity.AddPosListener(this);
        }

        public void UnRegisterListeners(){
            _entity.RemovePosListener(this);
        }

        public void OnPos(GameEntity entity, LVector2 newPosition){
            var dst = newPosition.ToVector3();
            //var src = transform.localPosition;
            //transform.localPosition = dst;// Vector3.Lerp(src, dst, 0.3f);
        }

        private void Update(){
            transform.localPosition = _entity.pos.value.ToVector3();
        }
    }
}