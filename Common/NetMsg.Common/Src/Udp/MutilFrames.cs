using System;
using System.Diagnostics;
using Lockstep.Serialization;

namespace NetMsg.Common {
    [SelfImplement]
    [Udp]
    [Serializable]
    public partial class MutilFrames : BaseFormater {
        public int startTick;
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.PutInt32(startTick);
            var count = (ushort) frames.Length;
            writer.PutUInt16(count);
            for (int i = 0; i < count; i++) {
                Debug.Assert(frames[i].tick == startTick + i, "Frame error");
                frames[i].BeforeSerialize();
                writer.PutBytes(frames[i].inputDatas);
            }
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt32();
            var tickCount = reader.GetUInt16();
            frames = new ServerFrame[tickCount];
            for (int i = 0; i < tickCount; i++) {
                var frame = new ServerFrame();
                frame.tick = startTick + i;
                frame.inputDatas = reader.GetBytes();
                frame.AfterDeserialize();
                frames[i] = frame;
            }
        }
    }
}