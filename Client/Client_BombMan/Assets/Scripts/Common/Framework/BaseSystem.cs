using Entitas;
using UnityEngine;

namespace Lockstep.Game {
    public class BaseSystem : ReferenceHolder, ISystem {
        public BaseSystem(Contexts contexts, IServiceContainer serviceContainer){
            InitReference(contexts);
            InitReference(serviceContainer);
        }
    }
}