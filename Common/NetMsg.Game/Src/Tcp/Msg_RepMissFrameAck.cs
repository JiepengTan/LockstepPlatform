using Lockstep.Serialization;

namespace NetMsg.Game {

    public partial class Msg_RepMissFrameAck : BaseFormater {
        public int missFrameTick;

        public override void Serialize(Serializer writer){
            writer.Put(missFrameTick);
        }

        public override void Deserialize(Deserializer reader){
            missFrameTick = reader.GetInt();
        }
    }
}