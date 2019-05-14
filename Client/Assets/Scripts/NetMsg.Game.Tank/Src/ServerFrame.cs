using Lockstep.Serialization;

namespace NetMsg.Game.Tank {
    
    public partial  class ServerFrame : BaseFormater {
        public uint tick;
        public PlayerInput[] inputs = new PlayerInput[0];

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
            inputs = new PlayerInput[len];
            for (int i = 0; i < len; i++) {
                inputs[i] = new PlayerInput();
                inputs[i].Deserialize(reader);
            }
        }

        public override string ToString(){
            var count = (inputs == null) ? 0 : inputs.Length;
            return $"t:{tick} inputNum:{count}";
        }

        public bool IsSame(ServerFrame frame){
            if (frame == null) return false;
            if (tick != frame.tick) return false;
            if (inputs.Length != frame.inputs.Length) {
                return false;
            }
            for (int i = 0; i < inputs.Length; i++) {
                var sInput = inputs[i];
                var oInput = frame.inputs[i];
                if((sInput == null) != (oInput == null)) return false;
                if (sInput == null) {
                    continue;
                }
                if (!sInput.IsSame(oInput)) {
                    return false;
                }
            }
            return true;
        }
    }





}