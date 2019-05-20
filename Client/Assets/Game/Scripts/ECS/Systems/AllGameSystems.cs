using Entitas;
using Lockstep.Game;
using Lockstep.Game.Features;
using Lockstep.Game.Systems;

sealed class AllGameSystems : Feature {
    public AllGameSystems(Contexts contexts, IServiceContainer services) : base("AllGameSystems"){
        Add(new GameStateFeature(contexts, services));
        Add(new InputFeature(contexts, services));
        Add(new GameFeature(contexts, services));
        Add(new CleanupFeature(contexts, services));
    }
}