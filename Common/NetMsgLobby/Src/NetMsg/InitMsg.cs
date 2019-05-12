using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial class InitMsg : NetMsgBase {
        public string name;

        public override void Serialize(Serializer writer){
            writer.Put(name);
        }

        public override void Deserialize(Deserializer reader){
            name = reader.GetString();
        }
    }
}