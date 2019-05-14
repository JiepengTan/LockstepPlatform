using Lockstep.Serialization;

namespace NetMsg.Game.Tank {
    public partial class Msg_CreateRoom :BaseFormater {
        public int roomType;
        public string name; 
        public override void Serialize(Serializer writer){
            writer.Put(roomType);
            writer.Put(name);
        }

        public override void Deserialize(Deserializer reader){
            roomType = reader.GetInt();
            name = reader.GetString();
        }
    }
}