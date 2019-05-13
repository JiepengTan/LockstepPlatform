#define DEBUG_FRAME_DELAY
using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using Lockstep.Core.Logic.Systems;
using Lockstep.Logging;
using Lockstep.Math;
using NetMsg.Game.Tank;

namespace Lockstep.Game {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class NotBackupAttribute : Attribute {
        public NotBackupAttribute(){ }
    }

    public class CopyableClass {
        [NotBackup] public byte iterIdx;

        public virtual CopyableClass DeepClone(){
            return null;
        }
    }

    public class Entity : CopyableClass { }

    public class Comp : CopyableClass {
        public virtual Entity DeepClone(){
            return null;
        }
    }

    public class GameState {
        public List<Entity> allEntities = new List<Entity>();

        /// <summary>
        /// 将整个游戏状态进行拷贝，包含引用的重定向
        /// </summary>
        public GameState Clone(){
            return null;
        }
    }

    public class World {
        public GameState curGameStatus;
        public Dictionary<uint, GameState> allBackGameState = new Dictionary<uint, GameState>();
        private byte[] actorIds;
        private List<BasePlayer> allPlayers = new List<BasePlayer>();
        public Action<BasePlayer> OnAddPlayer;

        public World(byte[] actorIds, bool isNeedRender){
            this.actorIds = actorIds;
        }

        public void DoStart(){
            foreach (byte actorId in actorIds) {
                var player = new BasePlayer(actorId, actorId.ToString());
                allPlayers.Add(player);
                if (OnAddPlayer != null) {
                    OnAddPlayer(player);
                }
            }
        }

        public void DoUpdate(LFloat deltaTime, ServerFrame frame){
            if (frame != null) {
#if DEBUG_FRAME_DELAY
                var time = 0;
                foreach (var input in frame.inputs) {
                    if (input.ActorId == Simulation.MainActorID) {
                        Lockstep.Logging.Debug.Log(
                            $"Tick {frame.tick} excute Delay {UnityEngine.Time.realtimeSinceStartup - input.timeSinceStartUp}");
                    }
                }
#endif
                foreach (var input in frame.inputs) {
                    var player = allPlayers[input.ActorId];
                    player?.ApplyInput(input, deltaTime);
                }
            }
        }

        public void DoDestroy(){ }

        
        public Contexts Contexts { get; }
        
        public uint Tick => Contexts.gameState.tick.value;
                                                         
        private readonly WorldSystems _systems;  

        public World(Contexts contexts, IEnumerable<byte> actorIds, params Feature[] features)
        {
            Contexts = contexts;                  

            foreach (var id in actorIds)
            {
                Contexts.actor.CreateEntity().AddId(id);
            }

            _systems = new WorldSystems(Contexts, features);
            _systems.Initialize();
        }

        public void Predict()
        {
            if (!Contexts.gameState.isPredicting)
            {
                Contexts.gameState.isPredicting = true;
            }

            Log.Trace(this, "Predict " + Contexts.gameState.tick.value);

            _systems.Execute();
            _systems.Cleanup();
        }

        public void Simulate()
        {
            if (Contexts.gameState.isPredicting)
            {
                Contexts.gameState.isPredicting = false;
            }

            Log.Trace(this, "Simulate " + Contexts.gameState.tick.value);

            _systems.Execute();
            _systems.Cleanup();

            var dbg = Contexts.debug.CreateEntity();
            dbg.AddTick(Tick);
            dbg.AddHashCode(Contexts.gameState.hashCode.value);
        }

        /// <summary>
        /// Reverts all changes that were done during or after the given tick
        /// </summary>
        public void RevertToTick(uint tick)
        {
            var snapshotIndices = Contexts.snapshot.GetEntities(SnapshotMatcher.Tick).Where(entity => entity.tick.value <= tick).Select(entity => entity.tick.value).ToList();
            var resultTick = snapshotIndices.Any() ? snapshotIndices.Max() : 0;

            Log.Info(this, "Rolling back from " + resultTick + " to " + Contexts.gameState.tick.value);

            /*
             * ====================== Revert actors ======================
             * most importantly: the entity-count per actor gets reverted so the composite key (Id + ActorId) of GameEntities stays in sync
             */

            var backedUpActors = Contexts.actor.GetEntities(ActorMatcher.Backup).Where(e => e.backup.tick == resultTick);
            foreach (var backedUpActor in backedUpActors)
            {
                var target = Contexts.actor.GetEntityWithId(backedUpActor.backup.actorId);

                //CopyTo does NOT remove additional existing components, so remove them first
                var additionalComponentIndices = target.GetComponentIndices().Except(
                    backedUpActor.GetComponentIndices()
                        .Except(new[] { ActorComponentsLookup.Backup })
                        .Concat(new[] { ActorComponentsLookup.Id }));

                foreach (var index in additionalComponentIndices)
                {
                    target.RemoveComponent(index);
                }

                backedUpActor.CopyTo(target, true, backedUpActor.GetComponentIndices().Except(new[] { ActorComponentsLookup.Backup }).ToArray());
            }

            /*
            * ====================== Revert game-entities ======================      
            */
            var currentEntities = Contexts.game.GetEntities(GameMatcher.LocalId);
            var backupEntities = Contexts.game.GetEntities(GameMatcher.Backup).Where(e => e.backup.tick == resultTick).ToList();
            var backupEntityIds = backupEntities.Select(entity => entity.backup.localEntityId);

            //Entities that were created in the prediction have to be destroyed
            var invalidEntities = currentEntities.Where(entity => !backupEntityIds.Contains(entity.localId.value)).ToList();
            foreach (var invalidEntity in invalidEntities)
            {
                invalidEntity.isDestroyed = true;
            }            

            foreach (var invalidBackupEntity in Contexts.game.GetEntities(GameMatcher.Backup).Where(e => e.backup.tick > resultTick))
            {                                                            
                invalidBackupEntity.Destroy();
            }


            foreach (var snapshotEntity in Contexts.snapshot.GetEntities(SnapshotMatcher.Tick).Where(e => e.tick.value > resultTick))
            {
                snapshotEntity.Destroy();
            }

            //Copy old state to the entity                                      
            foreach (var backupEntity in backupEntities)
            {
                var target = Contexts.game.GetEntityWithLocalId(backupEntity.backup.localEntityId);

                //CopyTo does NOT remove additional existing components, so remove them first
                var additionalComponentIndices = target.GetComponentIndices().Except(
                        backupEntity.GetComponentIndices()
                            .Except(new[] { GameComponentsLookup.Backup })
                            .Concat(new[] { GameComponentsLookup.LocalId }));

                foreach (var index in additionalComponentIndices)
                {
                    target.RemoveComponent(index);
                }

                backupEntity.CopyTo(target, true, backupEntity.GetComponentIndices().Except(new[] { GameComponentsLookup.Backup }).ToArray());
            }

            //TODO: restore locally destroyed entities   
            
            //Cleanup game-entities that are marked as destroyed
            _systems.Cleanup();

            Contexts.gameState.ReplaceTick(resultTick);

            while (Tick <= tick)
            {
                Simulate();
            }
        }      
    }
}