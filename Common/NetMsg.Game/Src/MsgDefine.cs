using Lockstep.Serialization;

namespace NetMsg.Game {
    [SelfImplement]
    [Udp]
    public partial class Msg_RepMissFrame : MutilFrames { }

    [SelfImplement]
    [Udp]
    public partial class Msg_ServerFrames : MutilFrames { }

    [Udp]
    public partial class Msg_HashCode : BaseFormater {
        public int startTick;
        public long[] hashCodes;
    }

    [Udp]
    public partial class Msg_RepMissFrameAck : BaseFormater {
        public int missFrameTick;
    }

    [Udp]
    public partial class Msg_ReqMissFrame : BaseFormater {
        public int startTick;
    }


    public partial class Msg_GameEvent : BaseFormater {
        public short type;
        [Limited] public byte[] content;
    }

    public partial class Msg_LoadingProgress : BaseFormater {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        public byte progress;
    }

    public partial class Msg_AllLoadingProgress : BaseFormater {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        [Limited(true)] public byte[] progress;

        public bool isAllDone;
    }

    public partial class Msg_PartFinished : BaseFormater {
        /// 关卡id
        public ushort level;
    }

    public partial class Msg_PlayerReady : BaseFormater {
        public int roomId;
    }

    public partial class Msg_RepInit : BaseFormater {
        public long playerId;
    }


    public partial class Msg_RoomInitMsg : BaseFormater {
        public string name;
    }

    public partial class Msg_StartRoomGame : BaseFormater {
        public int RoomID;
        public int Seed;
        public byte ActorID;
        [Limited(true)] public byte[] AllActors;
        public int SimulationSpeed;
    }
}