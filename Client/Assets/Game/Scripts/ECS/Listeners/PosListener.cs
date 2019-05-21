using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PosListener : MonoBehaviour, IEventListener, IPosListener {
        private GameEntity _entity;

        public void RegisterListeners(GameEntity entity){
            _entity = entity;
            _entity.AddPosListener(this);
        }

        public void UnregisterListeners(){
            _entity.RemovePosListener(this);
        }

        public void OnPos(GameEntity entity, Lockstep.Math.LVector2 newPosition){
            var dst = newPosition.ToVector3();
            var src = transform.localPosition;

            transform.localPosition = Vector3.Lerp(src, dst, 0.1f);
        }
    }
}