using System;
using System.Collections.Generic;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class ServerFrame : BaseFormater {
        public int tick;
        public Msg_PlayerInput[] inputs;

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
            tick = reader.GetInt();
            var len = reader.GetInt();
            inputs = new Msg_PlayerInput[len];
            for (int i = 0; i < len; i++) {
                inputs[i] = new Msg_PlayerInput();
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
            return inputs.EqualsEx(frame.inputs);
        }
    }
}