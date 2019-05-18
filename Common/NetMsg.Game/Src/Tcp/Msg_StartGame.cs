using Lockstep.Serialization;

namespace NetMsg.Game
{
    public class Msg_StartGame : ISerializable
    {
        public int RoomID { get; set; }
        public int Seed { get; set; }

        public byte ActorID { get; set; }

        public byte[] AllActors { get; set; }

        public int SimulationSpeed { get; set; } 
                                              
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
