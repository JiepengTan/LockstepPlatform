using Lockstep.Serialization;

namespace NetMsg.Game.Tank {
    public partial  class ReqMissFrame : BaseFormater {
        public uint[] missFrames;

        public override void Serialize(Serializer writer){
            writer.PutArray(missFrames);
        }

        public override void Deserialize(Deserializer reader){
            missFrames = reader.GetUIntArray();
        }
    }
}