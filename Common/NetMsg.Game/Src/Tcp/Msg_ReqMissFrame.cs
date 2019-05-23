using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_ReqMissFrame : BaseFormater {
        public bool isRequireAll;
        public int[] missFrames;

        public override void Serialize(Serializer writer){
            writer.Put(isRequireAll);
            writer.PutArray(missFrames);
        }

        public override void Deserialize(Deserializer reader){
            isRequireAll = reader.GetBool();
            missFrames = reader.GetIntArray();
        }
    }
}