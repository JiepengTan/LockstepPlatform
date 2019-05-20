using Lockstep.Game.Systems.GameState;

namespace Lockstep.Game.Features {
    sealed class GameStateFeature : Feature {
        public GameStateFeature(Contexts contexts, IServiceContainer services) : base("Game"){
            Add(new SystemInitState(contexts, services));
        }
    }
}