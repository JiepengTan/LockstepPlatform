using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public partial class ReqInit : NetMsgBase {
        public long playerId ;

        public override void Serialize(Serializer writer){
            writer.Put(playerId);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
        }
    }
}