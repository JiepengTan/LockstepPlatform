using Entitas;
using Lockstep.ECS.Systems.Actor;
using Lockstep.ECS.Systems.Debugging;
using Lockstep.ECS.Systems.GameState;
using Lockstep.Game;
using Lockstep.Game.Systems;

namespace Lockstep.ECS.Systems {
    public sealed class WorldSystems : Feature {
        public WorldSystems(Contexts contexts,IServiceContainer services, Feature logicFeature){
            Add(new InitializeEntityCount(contexts));
            // after game has init, backup before game logic
            Add(new OnNewPredictionCreateSnapshot(contexts));
            //game logic
            if (logicFeature != null) {
                Add(logicFeature);    
            }
            Add(new GameEventSystems(contexts));
            Add(new CalculateHashCode(contexts));
            //Performance-hit, only use for serious debugging
            //Add(new VerifyNoDuplicateBackups(contexts));             

            Add(new CleanDestroyedGameEntities(contexts));
            Add(new CleanDestroyedInputEntities(contexts));
            Add(new IncrementTick(contexts,services));
        }
    }
}