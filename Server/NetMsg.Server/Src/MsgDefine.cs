using Lockstep.Serialization;
using NetMsg.Common;


namespace NetMsg.Server {
    public partial class ServerIpInfo : BaseMsg {
        public ushort Port;
        public string Ip;
        public bool IsMaster;
        public byte ServerType;
    }

//IL
    public partial class Msg_I2L_UserLogin : BaseMsg {
        public string Account;
        public int GameType;
        public long UserId;
        public string LoginHash;
    }
    
//OMS
    public partial class Msg_ReqOtherServerInfo : BaseMsg {
        public byte ServerType;
        public byte DetailType;
    }

    public partial class Msg_RepOtherServerInfo : BaseMsg {
        public ServerIpInfo ServerInfo;
    }

//XS
    public partial class Msg_ReqServerInfo : BaseMsg {
        public ServerIpInfo ServerInfo;
    }
    public partial class Msg_ReqMasterInfo : BaseMsg {
        public ServerIpInfo ServerInfo;
    }

    public partial class Msg_RepMasterInfo : BaseMsg {
        public ServerIpInfo ServerInfo;
    }

    public partial class Msg_BorderMasterInfo : BaseMsg {
        public ServerIpInfo ServerInfo;
    }

//MS
    public partial class Msg_RegisterServer : BaseMsg {
        public ServerIpInfo ServerInfo;
    }

//DX
    public partial class Msg_RegisterDaemon : BaseMsg {
        public byte Type;
    }
}