using LitJson;


namespace Lockstep.Serialization {

    /// <summary>
    /// 不序列化到文件中
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field|System.AttributeTargets.Property, AllowMultiple = false)]
    public class NoGenCodeAttribute : System.Attribute { }


    [System.Serializable]
    [SelfImplement]
    public partial class BaseFormater : ISerializable, ISerializablePacket {
        public virtual void Serialize(Serializer writer){ }

        public virtual void Deserialize(Deserializer reader){}

        public override string ToString(){
            return JsonMapper.ToJson(this);
        }

        public byte[] ToBytes(){
            var writer = new Serializer();
            Serialize(writer);
            var bytes = writer.CopyData();// Compressor.Compress(writer.CopyData());
            return bytes;
        }

        public void FromBytes(byte[] data){
            var bytes = data;//Compressor.Decompress(data);
            var reader = new Deserializer(bytes);
            Deserialize(reader);
        }

        public static T FromBytes<T>(byte[] data) where T : BaseFormater, new(){
            var ret = new T();
            ret.FromBytes(data);
            return ret;
        }
    }

}