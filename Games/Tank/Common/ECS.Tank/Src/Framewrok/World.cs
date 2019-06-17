using System.Collections.Generic;
using System.Linq;
using Entitas;
using Lockstep.Logging;
using Lockstep.ECS.Systems;
using Lockstep.Game;

namespace Lockstep.ECS {
    public class World {

        public int Tick => _gameStateContext.tick.value;

        private readonly WorldSystems _systems;
        private ITimeMachineService _timeMachineService;

        private InputContext _inputContext;
        private ActorContext _actorContext;
        private GameContext _gameContext;
        private GameStateContext _gameStateContext;
        private SnapshotContext _snapshotContext;
        
        public World(Contexts contexts, IServiceContainer services, IEnumerable<byte> actorIds,
            Feature logicFeature){
            _actorContext = contexts.actor;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
            _snapshotContext= contexts.snapshot;
            
            _timeMachineService = services.GetService<ITimeMachineService>();
            _systems = new WorldSystems(contexts,services, logicFeature);
        }

        public void StartSimulate(){
            _systems.Initialize();
        }

        public void Predict(bool isNeedGenSnap = true){
            SetNeedGenSnapShot(isNeedGenSnap);
            Logger.Trace(this, "Predict " + _gameStateContext.tick.value);
            _timeMachineService.Backup(Tick);
            _systems.Execute();
            _systems.Cleanup();
        }

        private void SetNeedGenSnapShot(bool isNeedGenSnap){
            if (isNeedGenSnap) {
                if (Tick % FrameBuffer.SnapshotFrameInterval == 0) {
                    _gameStateContext.isBackupCurFrame = false; //确保一定会触发AddEvent
                    _gameStateContext.isBackupCurFrame = true;
                }
                else {
                    _gameStateContext.isBackupCurFrame = false;
                }
            }
            else {
                _gameStateContext.isBackupCurFrame = false;
            }
        }

        public void Simulate(bool isNeedGenSnap = true){
            SetNeedGenSnapShot(isNeedGenSnap);
            Logger.Trace(this, "Simulate " + _gameStateContext.tick.value);
            _timeMachineService.Backup(Tick);
            _systems.Execute();
            _systems.Cleanup();
            _timeMachineService.CurTick = Tick;
        }

        /// 清理无效的快照
        public void CleanUselessSnapshot(int checkedTick){
            if (checkedTick < 2) return;
            //_timeMachineService.Clean(checkedTick-1);
            var snapshotIndices = _snapshotContext.GetEntities(SnapshotMatcher.Tick)
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
            foreach (var invalidBackupEntity in _actorContext.GetEntities(ActorMatcher.Backup)
                .Where(e => e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }

            foreach (var invalidBackupEntity in _gameContext.GetEntities(GameMatcher.Backup)
                .Where(e => e.backup.tick < (resultTick))) {
                invalidBackupEntity.Destroy();
            }

            foreach (var snapshotEntity in _snapshotContext.GetEntities(SnapshotMatcher.Tick)
                .Where(e => e.tick.value < (resultTick))) {
                snapshotEntity.Destroy();
            }
            _systems.Cleanup();
        }

        /// <summary>
        /// Reverts all changes that were done during or after the given tick
        /// </summary>
        public void RollbackTo(int tick, int missFrameTick, bool isNeedClear = true){
            var snapshotIndices = _snapshotContext.GetEntities(SnapshotMatcher.Tick)
                .Where(entity => entity.tick.value <= tick).Select(entity => entity.tick.value).ToList();
            if (snapshotIndices.Count <= 0) return;
            snapshotIndices.Sort();
            Logging.Debug.Assert(snapshotIndices.Count > 0 && snapshotIndices[0] <= tick,
                $"Error! no correct history frame to revert minTick{(snapshotIndices.Count > 0 ? snapshotIndices[0] : 0)} targetTick {tick}");
            int i = snapshotIndices.Count - 1;
            for (; i >= 0; i--) {
                if (snapshotIndices[i] <= tick) {
                    break;
                }
            }

            var resultTick = snapshotIndices[i];
            if (resultTick == Tick) {
                Logging.Debug.Log("SelfTick should not rollback");
                return;
            }
            var snaps = "";
            foreach (var idx in snapshotIndices) {
                snaps += idx + " ";
            }

            Logging.Debug.Log(
                $"Rolling back {Tick}->{tick} :final from {resultTick} to {_gameStateContext.tick.value}  " +
                $"missTick:{missFrameTick} total:{Tick - resultTick} ");

            /*
             * ====================== Revert actors ======================
             * most importantly: the entity-count per actor gets reverted so the composite key (Id + ActorId) of GameEntities stays in sync
             */

            var backedUpActors =
                _actorContext.GetEntities(ActorMatcher.Backup).Where(e => e.backup.tick == resultTick);
            foreach (var backedUpActor in backedUpActors) {
                var target = _actorContext.GetEntityWithId(backedUpActor.backup.actorId);
                if (target == null) {
                    target = _actorContext.CreateEntity();
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
            var currentEntities = _gameContext.GetEntities(GameMatcher.LocalId);
            var backupEntities = _gameContext.GetEntities(GameMatcher.Backup).Where(e => e.backup.tick == resultTick)
                .ToList();
            var backupEntityIds = backupEntities.Select(entity => entity.backup.localEntityId);

            //Entities that were created in the prediction have to be destroyed
            var invalidEntities = currentEntities.Where(entity => !backupEntityIds.Contains(entity.localId.value))
                .ToList();
            foreach (var invalidEntity in invalidEntities) {
                invalidEntity.isDestroyed = true;
            }

            //将太后 和太前的snapshot 删除掉
            if (isNeedClear) {
                foreach (var invalidBackupEntity in _actorContext.GetEntities(ActorMatcher.Backup)
                    .Where(e => e.backup.tick != resultTick)) {
                    invalidBackupEntity.Destroy();
                }

                foreach (var invalidBackupEntity in _gameContext.GetEntities(GameMatcher.Backup)
                    .Where(e => e.backup.tick != resultTick)) {
                    invalidBackupEntity.Destroy();
                }

                foreach (var snapshotEntity in _snapshotContext.GetEntities(SnapshotMatcher.Tick)
                    .Where(e => e.tick.value != resultTick)) {
                    snapshotEntity.Destroy();
                }
            }


            //Copy old state to the entity                                      
            foreach (var backupEntity in backupEntities) {
                var target = _gameContext.GetEntityWithLocalId(backupEntity.backup.localEntityId);
                if (target == null) {
                    target = _gameContext.CreateEntity();
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
            _gameStateContext.ReplaceTick(resultTick);
            _timeMachineService.RollbackTo(resultTick);
            _timeMachineService.CurTick = resultTick;
        }
    }
}