using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial class InputCmd : NetMsgBase {
        public byte key;
        public LVector2 val;

        public override void Serialize(Serializer writer){
            writer.Put(key);
            writer.Put(val);
        }

        public override void Deserialize(Deserializer reader){
            key = reader.GetByte();
            val = reader.GetLVector2();
        }
    }
}