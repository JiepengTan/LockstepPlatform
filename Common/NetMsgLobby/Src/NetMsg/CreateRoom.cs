using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial class CreateRoom : NetMsgBase {
        public int type;
        public string name;

        public override void Serialize(Serializer writer){
            writer.Put(type);
            writer.Put(name);
        }

        public override void Deserialize(Deserializer reader){
            type = reader.GetInt();
            name = reader.GetString();
        }
    }
}