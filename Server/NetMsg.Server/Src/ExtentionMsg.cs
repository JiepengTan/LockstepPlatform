//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Server{
#if !DONT_USE_GENERATE_CODE

    public partial class Msg_BorderMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class Msg_I2L_UserLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(Account);
			writer.PutInt32(GameType);
			writer.PutString(LoginHash);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.GetString();
			GameType = reader.GetInt32();
			LoginHash = reader.GetString();
			UserId = reader.GetInt64();
        }
    }


    public partial class Msg_RegisterDaemon{
        public override void Serialize(Serializer writer){
			writer.PutByte(Type);
        }
    
        public override void Deserialize(Deserializer reader){
			Type = reader.GetByte();
        }
    }


    public partial class Msg_RegisterServer{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class Msg_RepMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class Msg_RepOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class Msg_ReqMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class Msg_ReqOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(DetailType);
			writer.PutByte(ServerType);
        }
    
        public override void Deserialize(Deserializer reader){
			DetailType = reader.GetByte();
			ServerType = reader.GetByte();
        }
    }


    public partial class Msg_ReqServerInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.Get(ref this.ServerInfo);
        }
    }


    public partial class ServerIpInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Ip);
			writer.PutBoolean(IsMaster);
			writer.PutUInt16(Port);
			writer.PutByte(ServerType);
        }
    
        public override void Deserialize(Deserializer reader){
			Ip = reader.GetString();
			IsMaster = reader.GetBoolean();
			Port = reader.GetUInt16();
			ServerType = reader.GetByte();
        }
    }


#endif
}
