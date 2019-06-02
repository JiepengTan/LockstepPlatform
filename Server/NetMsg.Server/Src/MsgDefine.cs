using Lockstep.Serialization;


namespace NetMsg.Server {
    public partial class ServerIpInfo : BaseFormater {
        public int port;
        public string ip;
        public bool isMaster;
        public byte serverType;
    }
//OMS
    public partial class Msg_ReqOtherServerInfo : BaseFormater {
        public byte serverType;
    }  
    public partial class Msg_RepOtherServerInfo : BaseFormater {
        public ServerIpInfo serverInfo;
    }  
//XS
    public partial class Msg_ReqMasterInfo : BaseFormater {
        public ServerIpInfo serverInfo;
    }

    public partial class Msg_RepMasterInfo : BaseFormater {
        public ServerIpInfo[] serverInfos;
    }

    public partial class Msg_BorderMasterInfo : BaseFormater {
        public ServerIpInfo serverInfo;
    }

//MS
    public partial class Msg_RegisterServer : BaseFormater {
        public ServerIpInfo serverInfo;
    }

//DX
    public partial class Msg_RegisterDaemon : BaseFormater {
        public byte type;
    }
}