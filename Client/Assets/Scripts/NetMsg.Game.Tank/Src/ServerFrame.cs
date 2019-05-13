using Lockstep.Serialization;

namespace NetMsg.Game.Tank {
    
    public partial  class ServerFrame : BaseFormater {
        public uint tick;
        public Input[] inputs = new Input[0];

        public override void Serialize(Serializer writer){
            writer.Put(tick);
            int count = 0;
            foreach (var input in inputs) {
                if (input != null) count++;
            }
            writer.Put(count);
            for (int i = 0; i < inputs.Length; i++) {
                inputs[i]?.Serialize(writer);
            }
        }

        public override void Deserialize(Deserializer reader){
            tick = reader.GetUInt();
            var len = reader.GetInt();
            inputs = new Input[len];
            for (int i = 0; i < len; i++) {
                inputs[i] = new Input();
                inputs[i].Deserialize(reader);
            }
        }

        public override string ToString(){
            var count = (inputs == null) ? 0 : inputs.Length;
            return $"t:{tick} inputNum:{count}";
        }
    }





}