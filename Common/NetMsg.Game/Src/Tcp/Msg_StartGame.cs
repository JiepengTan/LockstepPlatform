using System;
using Lockstep.Serialization;

namespace NetMsg.Game
{
    [Serializable]
    public class Msg_StartGame : ISerializable {
        public int RoomID;
        public int Seed;

        public byte ActorID;

        public byte[] AllActors;

        public int SimulationSpeed;
                                              
        public void Serialize(Serializer writer)
        {
            writer.Put(RoomID);
            writer.Put(Seed);
            writer.Put(ActorID);
            writer.PutBytesWithLength(AllActors);
            writer.Put(SimulationSpeed);
        }

        public void Deserialize(Deserializer reader)
        {
            RoomID = reader.GetInt();
            Seed = reader.GetInt();
            ActorID = reader.GetByte();
            AllActors = reader.GetBytesWithLength();
            SimulationSpeed = reader.GetInt();
        }
    }
}
