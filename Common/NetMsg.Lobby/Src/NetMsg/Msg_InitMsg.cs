using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class Msg_InitMsg : BaseFormater {
        public string account;

        public override void Serialize(Serializer writer){
            writer.Put(account);
        }

        public override void Deserialize(Deserializer reader){
            account = reader.GetString();
        }
    }
}