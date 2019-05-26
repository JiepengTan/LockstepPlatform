using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class Msg_ReqInit : BaseFormater {
        public long playerId;

        public override void Serialize(Serializer writer){
            writer.Put(playerId);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
        }
    }

    public partial class Msg_RepInit : BaseFormater {
        public long playerId;
        public string ip;
        public int port;
        public int roomId;
        public byte[] childMsg;
        public override void Serialize(Serializer writer){
            writer.Put(playerId);
            writer.Put(roomId);
            writer.Put(port);
            writer.Put(ip);
            writer.PutBytes_65535(childMsg);
        }

        public override void Deserialize(Deserializer reader){
            playerId = reader.GetLong();
            roomId = reader.GetInt();
            port = reader.GetInt();
            ip = reader.GetString();
            childMsg = reader.GetBytes_65535();
        }
    }

    public partial class Msg_CreateRoomResult : BaseFormater {
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