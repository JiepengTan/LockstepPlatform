using Lockstep.Game.Systems.Input;

namespace Lockstep.Game.Features {
    sealed class InputFeature : Feature {
        public InputFeature(Contexts contexts, IServiceContainer services) : base("Input"){
            Add(new SystemMoveInput(contexts, services));
            Add(new SystemFireInput(contexts, services));
        }
    }
}