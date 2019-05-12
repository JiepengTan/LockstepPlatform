using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class ReqInit : BaseFormater {
        public long playerId ;

        public override void Serialize(Serializer writer){
            writer.Put(playerId);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
        }
    }
}