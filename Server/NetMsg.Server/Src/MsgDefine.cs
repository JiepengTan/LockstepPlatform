using Lockstep.Serialization;


namespace NetMsg.Server {
    public partial class ServerIpInfo : BaseFormater {
        public ushort Port;
        public string Ip;
        public bool IsMaster;
        public byte ServerType;
    }

//IL
    public partial class Msg_I2L_UserLogin : BaseFormater {
        public string Account;
        public int GameType;
        public long UserId;
        public string LoginHash;
    }
    
//OMS
    public partial class Msg_ReqOtherServerInfo : BaseFormater {
        public byte ServerType;
        public byte DetailType;
    }

    public partial class Msg_RepOtherServerInfo : BaseFormater {
        public ServerIpInfo ServerInfo;
    }

//XS
    public partial class Msg_ReqServerInfo : BaseFormater {
        public ServerIpInfo ServerInfo;
    }
    public partial class Msg_ReqMasterInfo : BaseFormater {
        public ServerIpInfo ServerInfo;
    }

    public partial class Msg_RepMasterInfo : BaseFormater {
        public ServerIpInfo ServerInfo;
    }

    public partial class Msg_BorderMasterInfo : BaseFormater {
        public ServerIpInfo ServerInfo;
    }

//MS
    public partial class Msg_RegisterServer : BaseFormater {
        public ServerIpInfo ServerInfo;
    }

//DX
    public partial class Msg_RegisterDaemon : BaseFormater {
        public byte Type;
    }
}