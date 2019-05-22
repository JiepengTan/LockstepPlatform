using Entitas;
using Lockstep.Core.Logic.Systems.Actor;
using Lockstep.Core.Logic.Systems.Debugging;
using Lockstep.Core.Logic.Systems.GameState;
using Lockstep.Game.Systems;

namespace Lockstep.Core.Logic.Systems {
    public sealed class WorldSystems : Feature {
        public WorldSystems(Contexts contexts, Feature logicFeature){
            Add(new InitializeEntityCount(contexts));
            // after game has init, backup before game logic
            Add(new OnNewPredictionCreateSnapshot(contexts));
            Add(new CalculateHashCode(contexts));
            //game logic
            if (logicFeature != null) {
                Add(logicFeature);    
            }
            Add(new GameEventSystems(contexts));
            //Performance-hit, only use for serious debugging
            //Add(new VerifyNoDuplicateBackups(contexts));             

            Add(new CleanDestroyedGameEntities(contexts));
            Add(new CleanDestroyedInputEntities(contexts));
            Add(new IncrementTick(contexts));
        }
    }
}