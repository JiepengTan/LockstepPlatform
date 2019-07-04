//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Server{
#if !DONT_USE_GENERATE_CODE

    [System.Serializable]
    public partial class Msg_BorderMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_I2L_UserLogin{
        public override void Serialize(Serializer writer){
			writer.Write(Account);
			writer.Write(GameType);
			writer.Write(LoginHash);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.ReadString();
			GameType = reader.ReadInt32();
			LoginHash = reader.ReadString();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_RegisterDaemon{
        public override void Serialize(Serializer writer){
			writer.Write(Type);
        }
    
        public override void Deserialize(Deserializer reader){
			Type = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_RegisterServer{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_RepMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_RepOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_ReqMasterInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_ReqOtherServerInfo{
        public override void Serialize(Serializer writer){
			writer.Write(DetailType);
			writer.Write(ServerType);
        }
    
        public override void Deserialize(Deserializer reader){
			DetailType = reader.ReadByte();
			ServerType = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_ReqServerInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ServerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ServerInfo = reader.ReadRef(ref this.ServerInfo);
        }
    }


    [System.Serializable]
    public partial class ServerIpInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Ip);
			writer.Write(IsMaster);
			writer.Write(Port);
			writer.Write(ServerType);
        }
    
        public override void Deserialize(Deserializer reader){
			Ip = reader.ReadString();
			IsMaster = reader.ReadBoolean();
			Port = reader.ReadUInt16();
			ServerType = reader.ReadByte();
        }
    }


#endif
}
