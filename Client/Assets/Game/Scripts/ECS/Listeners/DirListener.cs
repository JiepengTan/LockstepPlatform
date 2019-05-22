using UnityEngine;

namespace Lockstep.Game {
    public class DirListener : MonoBehaviour, IEventListener, IDirListener {
        private GameEntity _entity;

        public void RegisterListeners(GameEntity entity){
            _entity = entity;
            _entity.AddDirListener(this);
        }

        public void UnregisterListeners(){
            _entity.RemoveDirListener(this);
        }

        public  void OnDir(GameEntity entity, EDir value){
            var deg = DirUtil.GetDirDeg(value);
            transform.localRotation = Quaternion.Euler(0,0,deg);
        }
    }
}