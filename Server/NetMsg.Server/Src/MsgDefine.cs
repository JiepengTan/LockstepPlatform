using Lockstep.Serialization;


namespace NetMsg.Server {
//XS
    public partial class Msg_ReqMasterInfo : BaseFormater {
        public byte type;
    }

    public partial class Msg_RepMasterInfo : BaseFormater {
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