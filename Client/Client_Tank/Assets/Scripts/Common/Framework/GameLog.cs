using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NetMsg.Common;

namespace Lockstep.Game {
    /// <summary>
    /// Stores all inputs including the tick in which the input was added. Can be used to exactly re-simulate a game (including rollback/prediction)
    /// </summary>
    [Serializable]
    public class GameLog {
        public byte LocalActorId { get; set; }
        public byte[] AllActorIds { get; set; }

        public Dictionary<int, Dictionary<int, Dictionary<byte, List<InputCmd>>>> InputLog { get; } =
            new Dictionary<int, Dictionary<int, Dictionary<byte, List<InputCmd>>>>();

        public void Add(int tick, int targetTick, byte actorId, List<InputCmd> commands){
            Add(tick, new Msg_PlayerInput(tick, actorId, commands));
        }

        public void Add(int tick, Msg_PlayerInput msg){
            if (!InputLog.ContainsKey(tick)) {
                InputLog.Add(tick, new Dictionary<int, Dictionary<byte, List<InputCmd>>>());
            }

            if (!InputLog[tick].ContainsKey(msg.Tick)) {
                InputLog[tick].Add(msg.Tick, new Dictionary<byte, List<InputCmd>>());
            }

            if (!InputLog[tick][msg.Tick].ContainsKey(msg.ActorId)) {
                InputLog[tick][msg.Tick].Add(msg.ActorId, new List<InputCmd>());
            }

            if (msg.Commands != null) {
                InputLog[tick][msg.Tick][msg.ActorId].AddRange(msg.Commands);
            }

        }

        public void WriteTo(Stream stream){
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }

        public static GameLog ReadFrom(Stream stream){
            IFormatter formatter = new BinaryFormatter();
            return (GameLog) formatter.Deserialize(stream);
        }
    }
}