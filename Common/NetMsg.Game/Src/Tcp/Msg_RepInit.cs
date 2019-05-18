using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_RepInit :BaseFormater{
        public long playerId; 
        public override void Serialize(Serializer writer){
            writer.Put(playerId);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
        }
    }
}