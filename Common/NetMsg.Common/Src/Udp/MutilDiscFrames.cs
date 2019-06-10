using Lockstep.Serialization;

namespace NetMsg.Common {
    [SelfImplement]
    [Udp]
    public partial class MutilDiscFrames : BaseFormater {
        public int startTick;
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.PutInt32(startTick);
            writer.PutArray(frames);
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt32();
            frames = reader.GetArray(frames);
        }
    }
}