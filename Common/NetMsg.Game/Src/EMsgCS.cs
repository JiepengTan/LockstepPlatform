namespace NetMsg.Game {
    //msg between client RoomServer
    public enum EMsgCS : byte {
        //Client GameServer
        //Input udp
        C2S_ReqMissFrame,
        C2S_RepMissFrameAck,
        S2C_RepMissFrame,
        C2S_HashCode,
        C2S_PlayerInput,
        S2C_FrameData,

        //GameEvent tcp
        C2S_GameEvent,
        S2C_GameEvent,
        //Room
        //废弃
        C2S_PlayerReady,
        S2C_PlayerReady,
        C2S_PlayerLeave,
        S2C_PlayerLeave,
        
        S2C_StartGame,

        //Level
        S2C_LoadingProgress,
        C2S_LoadingProgress,
        C2S_PartFinished,
        S2C_PartFinished,

        S2C_TickPlayer,
        EnumCount
    }


    public class NetworkDefine {
        /// <summary>
        /// 最大延迟时间 超过这个时间 依旧等不到玩家的输入包，默认玩家没有输入（输入丢失）
        /// </summary>
        public const int MAX_DELAY_TIME_MS = 300;

        /// 正常玩家的延迟
        public const int NORMAL_PLAYER_MAX_DELAY = 100;

        /// 正常玩家最大收到输入确认包的延迟 （有其中一个玩家输入延迟太大 且自身网络达到66%丢包率 情况下的时延）
        public const int MAX_FRAME_DATA_DELAY = MAX_DELAY_TIME_MS + NORMAL_PLAYER_MAX_DELAY + 2 * UPDATE_DELTATIME;

        /// 帧率
        public const int FRAME_RATE = 60;

        /// 每帧时间 (60帧) 
        public const int UPDATE_DELTATIME = 1000 / FRAME_RATE;

        public const string NetKey = "LockstepPlatform";
    }
}