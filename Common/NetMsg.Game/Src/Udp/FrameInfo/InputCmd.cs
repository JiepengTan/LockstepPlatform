using System;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class InputCmd : BaseFormater {
        public byte type;
        public byte[] content;

        public bool Equals(InputCmd cmdb){
            if (cmdb == null) return false;
            if (type != cmdb.type) return false;
            var arrb = cmdb.content;
            if ((content == null) != (arrb == null)) return false;
            if (content == null) return true;
            var count = arrb.Length;
            for (int i = 0; i < count; i++) {
                if (content[i] != arrb[i]) return false;
            }
            return true;
        }
        
        public override bool Equals(object obj){
            var cmdb = obj as InputCmd;
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
            var isNull = content == null;
            writer.Put(isNull);
            if(isNull) return;
            if (content.Length > byte.MaxValue) {
                throw new ArgumentOutOfRangeException($"Input Cmd len should less then {byte.MaxValue}");
            }
            writer.Put((byte) content.Length);
            writer.Put(content);
        }

        public override void Deserialize(Deserializer reader){
            type = reader.GetByte();
            var isNull = reader.GetBool();
            if (isNull) {
                content = null;
                return;
            }
            var len = reader.GetByte();
            content = new byte[len];
            reader.GetBytes(content, len);
        }
    }
}