using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial  class NetMsgBase : ISerializable {
        public virtual void Serialize(Serializer writer){}
        public virtual void Deserialize(Deserializer reader){}
    }
}