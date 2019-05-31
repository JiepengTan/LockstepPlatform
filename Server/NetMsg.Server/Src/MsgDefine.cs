using Lockstep.Serialization;


namespace NetMsg.Server {
//DS
    public class Msg_ReqMasterInfo : BaseFormater {
        public byte type;
    }

    public class Msg_RepMasterInfo : BaseFormater {
        public string ip;
        public int port;
    }

//MS
    public class Msg_RegisterServer : BaseFormater {
        public byte type;
    }

//DX
    public class Msg_RegisterDaemon : BaseFormater {
        public byte type;
    }
}