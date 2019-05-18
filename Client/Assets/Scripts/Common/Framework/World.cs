using System.Collections.Generic;
using System.Linq;
using Entitas;
using Lockstep.Logging;
using Lockstep.Core.Logic.Systems;
using Lockstep.Game;
using Debug = UnityEngine.Debug;

namespace Lockstep.Core.Logic {
    public class World {
        public Contexts Contexts { get; }

        public uint Tick => Contexts.gameState.tick.value;

        private readonly WorldSystems _systems;

        public World(Contexts contexts, IEnumerable<byte> actorIds, params Feature[] features){
            Contexts = contexts;

            foreach (var id in actorIds) {
                Contexts.actor.CreateEntity().AddId(id);
            }

            _systems = new WorldSystems(Contexts, features);
            _systems.Initialize();
        }

        public void Predict(){
            if (Tick % CommandBuffer.SNAPSHORT_FRAME_INTERVAL == 0) {
                Contexts.gameState.isPredicting = false;//确保一定会触发AddEvent
                Contexts.gameState.isPredicting = true;
            }

            Log.Trace(this, "Predict " + Contexts.gameState.tick.value);

            _systems.Execute();
            _systems.Cleanup();
        }

        public void Simulate(bool isNeedGenSnap = true){
            if ( isNeedGenSnap ) {
                if (Tick % CommandBuffer.SNAPSHORT_FRAME_INTERVAL == 0) {
                    Contexts.gameState.isPredicting = false;//确保一定会触发AddEvent
                    Contexts.gameState.isPredicting = true;
                }
            }
            else {
                Contexts.gameState.isPredicting = false;
            }

            Log.Trace(this, "Simulate " + Contexts.gameState.tick.value);

            _systems.Execute();
            _systems.Cleanup();
        }
        
        /// 清理无效的快照
        public void CleanUselessSnapshot(uint checkedTick){
            if(checkedTick < 2u) return;
            var snapshotIndices = Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(entity => entity.tick.value <= checkedTick).Select(entity => entity.tick.value).ToList();
            snapshotIndices.Sort();
            int i = snapshotIndices.Count - 1;
            for (; i >= 0; i--) {
                if (snapshotIndices[i] <= checkedTick) {
                    break;
                }
            }
            var resultTick = snapshotIndices[i];
            //将太后 和太前的snapshot 删除掉
            
            foreach (var invalidBackupEntity in Contexts.actor.GetEntities(ActorMatcher.Backup)
                .Where(e => e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }
            
            foreach (var invalidBackupEntity in Contexts.game.GetEntities(GameMatcher.Backup)
                .Where(e => e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }

            foreach (var snapshotEntity in Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(e => e.tick.value < (resultTick))) {
                snapshotEntity.Destroy();
            }
            _systems.Cleanup();
        }

        /// <summary>
        /// Reverts all changes that were done during or after the given tick
        /// </summary>
        public void RevertToTick(uint tick){
            var snapshotIndices = Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(entity => entity.tick.value <= tick).Select(entity => entity.tick.value).ToList();
            snapshotIndices.Sort();
            UnityEngine.Debug.Assert(snapshotIndices.Count > 0 && snapshotIndices[0] <= tick,
                $"Error! no correct history frame to revert minTick{(snapshotIndices.Count > 0 ? snapshotIndices[0] : 0u)} targetTick {tick}");
            int i = snapshotIndices.Count - 1;
            for (; i >= 0; i--) {
                if (snapshotIndices[i] <= tick) {
                    break;
                }
            }

            var resultTick = snapshotIndices[i];
            var snaps = "";
            foreach (var idx in snapshotIndices) {
                snaps +=  idx + " ";
            }

            Debug.Log( $"Rolling back {Tick}->{tick} :final from {resultTick} to {Contexts.gameState.tick.value}  total:{Tick - resultTick} snaps {snaps}" );

            /*
             * ====================== Revert actors ======================
             * most importantly: the entity-count per actor gets reverted so the composite key (Id + ActorId) of GameEntities stays in sync
             */

            var backedUpActors =
                Contexts.actor.GetEntities(ActorMatcher.Backup).Where(e => e.backup.tick == resultTick);
            foreach (var backedUpActor in backedUpActors) {
                var target = Contexts.actor.GetEntityWithId(backedUpActor.backup.actorId);

                //CopyTo does NOT remove additional existing components, so remove them first
                var additionalComponentIndices = target.GetComponentIndices().Except(
                    backedUpActor.GetComponentIndices()
                        .Except(new[] {ActorComponentsLookup.Backup})
                        .Concat(new[] {ActorComponentsLookup.Id}));

                foreach (var index in additionalComponentIndices) {
                    target.RemoveComponent(index);
                }

                backedUpActor.CopyTo(target, true,
                    backedUpActor.GetComponentIndices().Except(new[] {ActorComponentsLookup.Backup}).ToArray());
            }

            /*
            * ====================== Revert game-entities ======================      
            */
            var currentEntities = Contexts.game.GetEntities(GameMatcher.LocalId);
            var backupEntities = Contexts.game.GetEntities(GameMatcher.Backup).Where(e => e.backup.tick == resultTick)
                .ToList();
            var backupEntityIds = backupEntities.Select(entity => entity.backup.localEntityId);

            //Entities that were created in the prediction have to be destroyed
            var invalidEntities = currentEntities.Where(entity => !backupEntityIds.Contains(entity.localId.value))
                .ToList();
            foreach (var invalidEntity in invalidEntities) {
                invalidEntity.isDestroyed = true;
            }

            //将太后 和太前的snapshot 删除掉
            foreach (var invalidBackupEntity in Contexts.actor.GetEntities(ActorMatcher.Backup)
                .Where(e => e.backup.tick > resultTick ||
                            e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }
            
            foreach (var invalidBackupEntity in Contexts.game.GetEntities(GameMatcher.Backup)
                .Where(e => e.backup.tick > resultTick ||
                            e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }
            
            foreach (var snapshotEntity in Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(e => e.tick.value > resultTick ||
                            e.tick.value < (resultTick))) {
                snapshotEntity.Destroy();
            }

            //Copy old state to the entity                                      
            foreach (var backupEntity in backupEntities) {
                var target = Contexts.game.GetEntityWithLocalId(backupEntity.backup.localEntityId);

                //CopyTo does NOT remove additional existing components, so remove them first
                var additionalComponentIndices = target.GetComponentIndices().Except(
                    backupEntity.GetComponentIndices()
                        .Except(new[] {GameComponentsLookup.Backup})
                        .Concat(new[] {GameComponentsLookup.LocalId}));

                foreach (var index in additionalComponentIndices) {
                    target.RemoveComponent(index);
                }

                backupEntity.CopyTo(target, true,
                    backupEntity.GetComponentIndices().Except(new[] {GameComponentsLookup.Backup}).ToArray());
            }

            //TODO: restore locally destroyed entities   

            //Cleanup game-entities that are marked as destroyed
            _systems.Cleanup();
            Contexts.gameState.ReplaceTick(resultTick);
            var selectedEntities = Contexts.game.GetEntities(GameMatcher.LocalId);
            foreach (var entity in selectedEntities)
            {
                if (!entity.isDestroyed) {
                    var pos = entity.position.value;
                    var preNum = SimulationManager.allAccumInputCount[entity.actorId.value, resultTick-1] * 16;
                    var nextNum = SimulationManager.allAccumInputCount[entity.actorId.value, resultTick] * 16;
                }
            }
        }
    }
}