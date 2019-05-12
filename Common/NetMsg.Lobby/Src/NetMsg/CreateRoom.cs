using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class CreateRoom : BaseFormater {
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