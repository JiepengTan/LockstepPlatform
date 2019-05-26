using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_ReqMissFrame : BaseFormater {
        public int startTick;

        public override void Serialize(Serializer writer){
            writer.Put(startTick);
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt();
        }
    }
}