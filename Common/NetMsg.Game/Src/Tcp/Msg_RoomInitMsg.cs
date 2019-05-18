using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_RoomInitMsg :BaseFormater{
        public string name; 
        public override void Serialize(Serializer writer){
            writer.Put(name);
        }

        public override void Deserialize(Deserializer reader){
            name = reader.GetString();
        }
    }
}