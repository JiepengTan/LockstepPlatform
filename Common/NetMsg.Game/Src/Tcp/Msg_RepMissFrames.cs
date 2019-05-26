#define DEBUG_FRAME_DELAY
using System;
using System.Diagnostics;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class MutilDiscFrames : BaseFormater {
        public int startTick;
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.Put(startTick);
            writer.PutArray(frames);
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt();
            frames = reader.GetArray<ServerFrame>();
        }
    }

    [System.Serializable]
    public partial class MutilFrames : BaseFormater {
        public int startTick;
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.Put(startTick);
            var count = (ushort) frames.Length;
            writer.Put(count);
            for (int i = 0; i < count; i++) {
                Debug.Assert(frames[i].tick == startTick + i, "Frame error");
                frames[i].BeforeSerialize();
                writer.PutBytes_65535(frames[i].inputDatas);
            }
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt();
            var tickCount = reader.GetUShort();
            frames = new ServerFrame[tickCount];
            for (int i = 0; i < tickCount; i++) {
                var frame = new ServerFrame();
                frame.tick = startTick + i;
                frame.inputDatas = reader.GetBytes_65535();
                frame.AfterDeserialize();
                frames[i] = frame;
            }
        }
    }

    public partial class Msg_RepMissFrame : MutilFrames { }
}