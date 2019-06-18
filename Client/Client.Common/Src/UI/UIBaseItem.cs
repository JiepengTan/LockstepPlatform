using Lockstep.Game.UI;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public abstract class UIBaseItem : MonoBehaviour {
        protected IReferenceHolder _referenceHolder;
        protected T GetRef<T>(string name) where T : UnityEngine.Object{
            return _referenceHolder.GetRef<T>(name);
        }

        protected virtual void Awake(){
            _referenceHolder = GetComponent<IReferenceHolder>();
            Debug.Assert(_referenceHolder != null, GetType() + " miss IReferenceHolder ");
            DoAwake();
        }
        protected virtual void DoAwake(){ }
    }
}