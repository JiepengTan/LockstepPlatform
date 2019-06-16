using Entitas;

namespace Lockstep.Game {
    public class BaseSystem : BaseSystemReferenceHolder, ISystem {
        public BaseSystem(Contexts contexts, IServiceContainer serviceContainer){
            InitReference(contexts);
            InitReference(serviceContainer);
        }
    }
}