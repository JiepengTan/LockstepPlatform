using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class Msg_CreateRoom : BaseFormater {
        public byte type;
        public byte size;
        public string name;

        public override void Serialize(Serializer writer){
            writer.Put(type);
            writer.Put(size);
            writer.Put(name);
        }

        public override void Deserialize(Deserializer reader){
            type = reader.GetByte();
            size = reader.GetByte();
            name = reader.GetString();
        }
    }
    
    public partial class Msg_JoinRoom : BaseFormater {
        public int roomId;

        public override void Serialize(Serializer writer){
            writer.Put(roomId);
        }

        public override void Deserialize(Deserializer reader){
            roomId = reader.GetInt();
        }
    }
}