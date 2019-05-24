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

        public int Tick => Contexts.gameState.tick.value;

        private readonly WorldSystems _systems;
        private ITimeMachineService _timeMachineService;

        public World(Contexts contexts, ITimeMachineService timeMachineService, IEnumerable<byte> actorIds,
            Feature logicFeature){
            Contexts = contexts;
            _timeMachineService = timeMachineService;
            _systems = new WorldSystems(Contexts, logicFeature);
        }

        public void StartSimulate(){
            _systems.Initialize();
        }

        public void Predict(bool isNeedGenSnap = true){
            SetNeedGenSnapShot(isNeedGenSnap);
            Log.Trace(this, "Predict " + Contexts.gameState.tick.value);
            _timeMachineService.Backup(Tick);
            _systems.Execute();
            _systems.Cleanup();
        }

        private void SetNeedGenSnapShot(bool isNeedGenSnap){
            if (isNeedGenSnap) {
                if (Tick % FrameBuffer.SNAPSHORT_FRAME_INTERVAL == 0) {
                    Contexts.gameState.isBackupCurFrame = false; //确保一定会触发AddEvent
                    Contexts.gameState.isBackupCurFrame = true;
                }
            }
            else {
                Contexts.gameState.isBackupCurFrame = false;
            }
        }

        public void Simulate(bool isNeedGenSnap = true){
            SetNeedGenSnapShot(isNeedGenSnap);
            Log.Trace(this, "Simulate " + Contexts.gameState.tick.value);
            _timeMachineService.Backup(Tick);
            _systems.Execute();
            _systems.Cleanup();
        }

        /// 清理无效的快照
        public void CleanUselessSnapshot(int checkedTick){
            if (checkedTick < 2) return;
            //_timeMachineService.Clean(checkedTick-1);
            var snapshotIndices = Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(entity => entity.tick.value <= checkedTick).Select(entity => entity.tick.value).ToList();
            if (snapshotIndices.Count == 0) return;
            snapshotIndices.Sort();
            int i = snapshotIndices.Count - 1;
            for (; i >= 0; i--) {
                if (snapshotIndices[i] <= checkedTick) {
                    break;
                }
            }

            if (i < 0) return;
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
        public void RollbackTo(int tick,int missFrameTick){
            var snapshotIndices = Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(entity => entity.tick.value <= tick).Select(entity => entity.tick.value).ToList();
            snapshotIndices.Sort();
            UnityEngine.Debug.Assert(snapshotIndices.Count > 0 && snapshotIndices[0] <= tick,
                $"Error! no correct history frame to revert minTick{(snapshotIndices.Count > 0 ? snapshotIndices[0] : 0)} targetTick {tick}");
            int i = snapshotIndices.Count - 1;
            for (; i >= 0; i--) {
                if (snapshotIndices[i] <= tick) {
                    break;
                }
            }

            var resultTick = snapshotIndices[i];
            var snaps = "";
            foreach (var idx in snapshotIndices) {
                snaps += idx + " ";
            }

            Debug.Log(
                $"Rolling back {Tick}->{tick} :final from {resultTick} to {Contexts.gameState.tick.value}  missTick:{missFrameTick} total:{Tick - resultTick} snaps {snaps}");

            /*
             * ====================== Revert actors ======================
             * most importantly: the entity-count per actor gets reverted so the composite key (Id + ActorId) of GameEntities stays in sync
             */

            var backedUpActors =
                Contexts.actor.GetEntities(ActorMatcher.Backup).Where(e => e.backup.tick == resultTick);
            foreach (var backedUpActor in backedUpActors) {
                var target = Contexts.actor.GetEntityWithId(backedUpActor.backup.actorId);
                if (target == null) {
                    target = Contexts.actor.CreateEntity();
                    target.AddId(backedUpActor.backup.actorId);
                }

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
                .Where(e => e.backup.tick != resultTick)) {
                invalidBackupEntity.Destroy();
            }

            foreach (var invalidBackupEntity in Contexts.game.GetEntities(GameMatcher.Backup)
                .Where(e => e.backup.tick != resultTick)) {
                invalidBackupEntity.Destroy();
            }

            foreach (var snapshotEntity in Contexts.snapshot.GetEntities(SnapshotMatcher.Tick)
                .Where(e => e.tick.value != resultTick)) {
                snapshotEntity.Destroy();
            }

            //Copy old state to the entity                                      
            foreach (var backupEntity in backupEntities) {
                var target = Contexts.game.GetEntityWithLocalId(backupEntity.backup.localEntityId);
                if (target == null) {
                    target = Contexts.game.CreateEntity();
                    target.AddLocalId(backupEntity.backup.localEntityId);
                }

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
            _timeMachineService.RollbackTo(resultTick);
            Contexts.gameState.ReplaceTick(resultTick);
        }
    }
}