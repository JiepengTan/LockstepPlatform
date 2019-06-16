using Entitas;
using Lockstep.Game;
using Lockstep.Game.Features;
using Lockstep.Game.Systems;

public class GameLogicSystems : Feature {
    public GameLogicSystems(Contexts contexts, IServiceContainer services) : base("AllGameSystems"){
        //Init
        Add(new GameInitStateFeature(contexts, services));
        //Input
        Add(new InputFeature(contexts, services));
        //Logic
        Add(new GameFeature(contexts, services));
        //Clean
        Add(new CleanupFeature(contexts, services));
    }
}