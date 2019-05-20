using Entitas;
using UnityEngine;

namespace Lockstep.Game.Systems {
    public class BaseSystem : ReferenceHolder, ISystem {
        public BaseSystem(Contexts contexts, IServiceContainer serviceContainer){
            base.InitReference(contexts);
            base.InitReference(serviceContainer);
        }
    }
}