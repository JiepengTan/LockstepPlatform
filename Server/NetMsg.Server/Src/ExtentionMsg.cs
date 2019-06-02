//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Server{
#if !DONT_USE_GENERATE_CODE

    public partial class Msg_BorderMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Put(serverInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			serverInfo = reader.Get(ref this.serverInfo);
        }
    }


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
			writer.Put(serverInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			serverInfo = reader.Get(ref this.serverInfo);
        }
    }


    public partial class Msg_RepMasterInfo{
        public override void Serialize(Serializer writer){
			writer.PutArray(serverInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			serverInfos = reader.GetArray(ref this.serverInfos);
        }
    }


    public partial class Msg_RepOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.Put(serverInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			serverInfo = reader.Get(ref this.serverInfo);
        }
    }


    public partial class Msg_ReqMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Put(serverInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			serverInfo = reader.Get(ref this.serverInfo);
        }
    }


    public partial class Msg_ReqOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(serverType);
        }
    
        public override void Deserialize(Deserializer reader){
			serverType = reader.GetByte();
        }
    }


    public partial class ServerIpInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(ip);
			writer.PutBoolean(isMaster);
			writer.PutInt32(port);
			writer.PutByte(serverType);
        }
    
        public override void Deserialize(Deserializer reader){
			ip = reader.GetString();
			isMaster = reader.GetBoolean();
			port = reader.GetInt32();
			serverType = reader.GetByte();
        }
    }


#endif
}
