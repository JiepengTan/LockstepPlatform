using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_LoadingProgress :BaseFormater{
        /// <summary>
        /// [进度百分比 1表示1% 100表示已经加载完成]
        /// </summary>
        public byte progress; 
        public override void Serialize(Serializer writer){
            writer.Put(progress);
        }

        public override void Deserialize(Deserializer reader){
            progress = reader.GetByte();
        }
    }
}