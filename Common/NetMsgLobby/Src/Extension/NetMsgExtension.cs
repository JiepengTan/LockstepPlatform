using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby {
    public static class NetMsgExtension {
        public static void RegisterAllNetMsg(){ }

        public static T Parse<T>(this Deserializer reader) where T : ISerializable,new (){
            var val = new T();
            val.Deserialize(reader);
            return val;
        }
    }
}