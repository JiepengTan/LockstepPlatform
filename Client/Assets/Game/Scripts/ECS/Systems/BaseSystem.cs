using UnityEngine;

namespace Lockstep.Game.Systems {
    public class BaseSystem : ReferenceHolder {
        public void InitReference(Contexts contexts, IServiceContainer serviceContainer){
            base.InitReference(contexts);
            base.InitReference(serviceContainer);
        }
    }
}