#define DEBUG_FRAME_DELAY
using System;
using System.Collections.Generic;
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
        public uint Tick;
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
                    if (input.playerID == Simulation.MainActorID) {
                        Lockstep.Logging.Debug.Log(
                            $"Tick {frame.tick} excute Delay {UnityEngine.Time.realtimeSinceStartup - input.timeSinceStartUp}");
                    }
                }
#endif
                foreach (var input in frame.inputs) {
                    var player = allPlayers[input.playerID];
                    player?.ApplyInput(input, deltaTime);
                }
            }
        }

        public void DoDestroy(){ }

        public void Predict(){ }

        public void RevertToTick(uint tick){ }
    }
}