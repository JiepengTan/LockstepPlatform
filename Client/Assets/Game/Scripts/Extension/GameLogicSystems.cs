using Entitas;
using Lockstep.Game;
using Lockstep.Game.Features;
using Lockstep.Game.Systems;

sealed class GameLogicSystems : Feature {
    public GameLogicSystems(Contexts contexts, IServiceContainer services) : base("AllGameSystems"){
        Add(new GameStateFeature(contexts, services));
        Add(new InputFeature(contexts, services));
        Add(new GameFeature(contexts, services));
        Add(new CleanupFeature(contexts, services));
    }
}