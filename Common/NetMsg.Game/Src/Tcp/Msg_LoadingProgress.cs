using System;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_LoadingProgress : BaseFormater {
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

    public partial class Msg_AllLoadingProgress : BaseFormater {
        /// <summary>
        /// [进度百分比 1表示1% 100表示已经加载完成]
        /// </summary>
        public byte[] progress;
        public bool isAllDone;

        public override void Serialize(Serializer writer){
            writer.Put(isAllDone);
            writer.PutArray_65535(progress);
        }

        public override void Deserialize(Deserializer reader){
            isAllDone = reader.GetBool();
            progress = reader.GetBytes_65535();
        }
    }
}