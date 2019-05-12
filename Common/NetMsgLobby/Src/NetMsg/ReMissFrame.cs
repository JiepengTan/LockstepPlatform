using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial  class ReqMissFrame : NetMsgBase {
        public uint[] missFrames;

        public override void Serialize(Serializer writer){
            writer.PutArray(missFrames);
        }

        public override void Deserialize(Deserializer reader){
            missFrames = reader.GetUIntArray();
        }
    }
}