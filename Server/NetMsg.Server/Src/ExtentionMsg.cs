//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Server{
#if !DONT_USE_GENERATE_CODE

    public partial class Msg_RegisterDaemon{
        public override void Serialize(Serializer writer){
			writer.PutByte(type);
        }
    
        public override void Deserialize(Deserializer reader){
			type = reader.GetByte();
        }
    }


    public partial class Msg_RegisterServer{
        public override void Serialize(Serializer writer){
			writer.PutByte(type);
        }
    
        public override void Deserialize(Deserializer reader){
			type = reader.GetByte();
        }
    }


    public partial class Msg_RepMasterInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(ip);
			writer.PutInt32(port);
        }
    
        public override void Deserialize(Deserializer reader){
			ip = reader.GetString();
			port = reader.GetInt32();
        }
    }


    public partial class Msg_ReqMasterInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(type);
        }
    
        public override void Deserialize(Deserializer reader){
			type = reader.GetByte();
        }
    }


#endif
}
