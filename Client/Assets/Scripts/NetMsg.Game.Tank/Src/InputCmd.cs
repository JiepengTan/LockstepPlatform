using Lockstep.Serialization;

namespace NetMsg.Game.Tank {
    public partial class InputCmd : BaseFormater {
        public int key;
        public int val1;
        public int val2;

        public override void Serialize(Serializer writer){
            writer.Put(key);
            writer.Put(val1);
            writer.Put(val2);
        }

        public override void Deserialize(Deserializer reader){
            key = reader.GetInt();
            val1 = reader.GetInt();
            val2 = reader.GetInt();
        }
    }
}