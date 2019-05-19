using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_PartFinished :BaseFormater{
        /// <summary>
        /// 关卡id
        /// </summary>
        public ushort level; 
        public override void Serialize(Serializer writer){
            writer.Put(level);
        }

        public override void Deserialize(Deserializer reader){
            level = reader.GetUShort();
        }
    }
}