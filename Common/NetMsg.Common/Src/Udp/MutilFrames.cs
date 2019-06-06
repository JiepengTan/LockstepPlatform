using System.Diagnostics;
using Lockstep.Serialization;

namespace NetMsg.Common {
    [SelfImplement]
    [Udp]
    public partial class MutilFrames : BaseFormater {
        public int StartTick;
        public ServerFrame[] Frames;

        public override void Serialize(Serializer writer){
            writer.PutInt32(StartTick);
            var count = (ushort) Frames.Length;
            writer.PutUInt16(count);
            for (int i = 0; i < count; i++) {
                Debug.Assert(Frames[i].Tick == StartTick + i, "Frame error");
                Frames[i].BeforeSerialize();
                writer.PutBytes(Frames[i].InputDatas);
            }
        }

        public override void Deserialize(Deserializer reader){
            StartTick = reader.GetInt32();
            var tickCount = reader.GetUInt16();
            Frames = new ServerFrame[tickCount];
            for (int i = 0; i < tickCount; i++) {
                var frame = new ServerFrame();
                frame.Tick = StartTick + i;
                frame.InputDatas = reader.GetBytes();
                frame.AfterDeserialize();
                Frames[i] = frame;
            }
        }
    }
}