using Lockstep.Serialization;

namespace NetMsg.Game {
    public class Msg_HashCode : BaseFormater {
        public int startTick;
        public long[] hashCodes;

        public override void Serialize(Serializer writer){
            writer.Put(startTick);
            writer.PutArray(hashCodes);
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.GetInt();
            hashCodes = reader.GetLongArray();
        }
    }
}