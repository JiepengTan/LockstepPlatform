using Lockstep.Serialization;


namespace NetMsg.Server {
//XS
    public partial class Msg_ReqMasterInfo : BaseFormater {
        public int masterPort;
        public string ip;
        public bool isMaster;
        public byte serverType;
    }

    public partial class Msg_RepMasterInfo : BaseFormater {
        public byte serverType;
        public string ip;
        public int port;
    }

//MS
    public partial class Msg_RegisterServer : BaseFormater {
        public byte type;
    }

//DX
    public partial class Msg_RegisterDaemon : BaseFormater {
        public byte type;
    }
}