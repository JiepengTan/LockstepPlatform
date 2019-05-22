using System;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_GameEvent : BaseFormater {
        public short type;
        public byte[] content;

        public bool Equals(Msg_GameEvent cmdb){
            if (cmdb == null) return false;
            if (type != cmdb.type) return false;
            return content.EqualsEx(cmdb.content);
        }
        
        public override bool Equals(object obj){
            var cmdb = obj as Msg_GameEvent;
            return Equals(cmdb);
        }

        public override int GetHashCode(){
            return type;
        }

        public override string ToString(){
            return $"t:{type} content:{content?.Length ?? 0}";
        }
        
        public override void Serialize(Serializer writer){
            writer.Put(type);
            writer.PutArray_65535(content);
        }

        public override void Deserialize(Deserializer reader){
            type = reader.GetByte();
            content = reader.GetBytes_65535();
        }
    }
}