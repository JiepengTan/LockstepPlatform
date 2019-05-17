using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class Msg_ReqInit : BaseFormater {
        public long playerId ;

        public override void Serialize(Serializer writer){
            writer.Put(playerId);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
        }
    }

    public partial class Msg_CreateRoomResult :BaseFormater{
        public string ip;
        public int port;
        public int roomId;
        
        public override void Serialize(Serializer writer){
            writer.Put(roomId);
            writer.Put(port);
            writer.Put(ip);
        }

        public override void Deserialize(Deserializer reader){
            roomId = reader.GetInt();
            port = reader.GetInt();
            ip = reader.GetString();
        }
    }
}