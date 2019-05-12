using Lockstep.Serialization;

namespace Lockstep.Serialization {
    public static class NetMsgExtension {

        public static T Parse<T>(this Deserializer reader) where T : ISerializable,new (){
            var val = new T();
            val.Deserialize(reader);
            return val;
        }
    }
}