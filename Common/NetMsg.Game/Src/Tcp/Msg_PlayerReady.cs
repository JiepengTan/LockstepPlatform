using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_PlayerReady :BaseFormater{
        public int roomId; 
        public override void Serialize(Serializer writer){
            writer.Put(roomId);
        }

        public override void Deserialize(Deserializer reader){
            roomId = reader.GetInt();
        }
    }
}