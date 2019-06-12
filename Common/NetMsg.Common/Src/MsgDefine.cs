using System;
using Lockstep.Serialization;

namespace NetMsg.Common {
    #region UDP

    [Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_RepMissFrame : MutilFrames { }

    [Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_ServerFrames : MutilFrames { }

    [Udp]
    public partial class Msg_HashCode : BaseFormater {
        public int StartTick;
        public long[] HashCodes;
    }

    [Udp]
    public partial class Msg_RepMissFrameAck : BaseFormater {
        public int MissFrameTick;
    }

    [Udp]
    public partial class Msg_ReqMissFrame : BaseFormater {
        public int StartTick;
    }

    #endregion

    #region TCP

    public partial class IPEndInfo : BaseFormater {
        public string Ip;
        public ushort Port;
    }

    public partial class RoomChangedInfo : BaseFormater {
        public int RoomId;
        public byte CurPlayerCount;
    }

    public partial class RoomInfo : BaseFormater {
        public int GameType;
        public int MapId;
        public string Name;
        public byte MaxPlayerCount;

        public int RoomId;
        public byte State;
        public string OwnerName;
        public byte CurPlayerCount;
    }

    public partial class UserGameInfo : BaseFormater {
        public string Name;
        public byte[] Data;
    }

    public partial class GamePlayerInfo : BaseFormater {
        public long UserId;
        public string Account;
        public string LoginHash;
    }

    public partial class RoomPlayerInfo : BaseFormater {
        public long UserId;
        public string Name;
        public byte Status;
    }

    public partial class RoomChatInfo : BaseFormater {
        public byte Channel;
        public long SrcUserId;
        public long DstUserId;
        public byte[] Message;
    }

//IC
    public partial class Msg_C2I_UserLogin : BaseFormater {
        public string Account;
        public string Password;
        public string EncryptHash;
        public int GameType;
    }

    public partial class Msg_I2C_LoginResult : BaseFormater {
        public byte LoginResult;
        public string LoginHash;
        public long UserId;
        public IPEndInfo LobbyEnd;
    }


//LC
    public partial class Msg_S2C_TickPlayer : BaseFormater {
        public byte Reason;
    }

    public partial class Msg_C2L_UserLogin : BaseFormater {
        public long UserId;
        public string LoginHash;
    }


    public partial class Msg_C2L_ReqRoomList : BaseFormater {
        public short StartIdx;
    }

    public partial class Msg_L2C_RoomList : BaseFormater {
        public int GameType;
        public RoomInfo[] Rooms;
    }

    public partial class Msg_L2C_RoomInfoUpdate : BaseFormater {
        public RoomInfo[] AddInfo;
        public int[] DeleteInfo;
        public RoomChangedInfo[] ChangedInfo;
    }


    public partial class Msg_C2L_JoinRoom : BaseFormater {
        public int RoomId;
    }

    public partial class Msg_L2C_JoinRoomResult : BaseFormater {
        public RoomPlayerInfo[] PlayerInfos;
    }

    public partial class Msg_C2L_ReadyInRoom : BaseFormater {
        public byte State;
    }

    public partial class Msg_L2C_ReadyInRoom : BaseFormater {
        public long UserId;
        public byte State;
    }

    public partial class Msg_L2C_JoinRoom : BaseFormater {
        public RoomPlayerInfo PlayerInfo;
    }

    public partial class Msg_C2L_LeaveRoom : BaseFormater {
        public byte Reason;
    }

    public partial class Msg_L2C_LeaveRoom : BaseFormater {
        public long UserId;
    }

    public partial class Msg_C2L_RoomChatInfo : BaseFormater {
        public RoomChatInfo ChatInfo;
    }

    public partial class Msg_L2C_RoomChatInfo : BaseFormater {
        public RoomChatInfo ChatInfo;
    }

    public partial class Msg_C2L_CreateRoom : BaseFormater {
        public int GameType;
        public int MapId;
        public string Name;
        public byte MaxPlayerCount;
    }

    public partial class Msg_L2C_CreateRoom : BaseFormater {
        public RoomInfo Info;
        public RoomPlayerInfo[] PlayerInfos;
    }

    public partial class Msg_C2L_StartGame : BaseFormater {
        public byte Reason;
    }

    public partial class Msg_L2C_StartGame : BaseFormater {
        public byte Result;
        public IPEndInfo GameServerEnd;
        public string GameHash;
        public int GameId;
        public int RoomId;
        public bool IsReconnect;
    }

//LG
    public partial class Msg_L2G_UserReconnect : BaseFormater {
        public GamePlayerInfo PlayerInfo;
    }

    public partial class Msg_L2G_CreateGame : BaseFormater {
        public int GameType;
        public int MapId;
        public int RoomId;
        public GamePlayerInfo[] Players;
        public string GameHash;
    }

    public partial class Msg_L2G_UserLeave : BaseFormater {
        public long UserId;
        public int GameId;
    }

    public partial class Msg_G2L_OnGameFinished : BaseFormater {
        public int GameId;
        public int RoomId;
    }

//GC
    public partial class MessageHello : BaseFormater {
        public GamePlayerInfo UserInfo;
        public int GameType;
        public string GameHash;
        public int GameId;
        public bool IsReconnect;
    }

    public partial class Msg_C2G_Hello : BaseFormater {
        public MessageHello Hello;
    }

    public partial class Msg_G2C_Hello : BaseFormater {
        public byte LocalId;
        public byte UserCount;
        public int MapId;
        public int GameId;
        public int Seed;
        public IPEndInfo UdpEnd;
    }

    public partial class Msg_G2C_GameStartInfo : BaseFormater {
        public byte UserCount;
        public int MapId;
        public int RoomId;
        public int Seed;
        public GameData[] UserInfos;
        public IPEndInfo UdpEnd;
        public IPEndInfo TcpEnd;
        public int SimulationSpeed;
    }

    public partial class Msg_C2G_UdpHello : BaseFormater {
        public MessageHello Hello;
    }

    public partial class Msg_G2C_GameStatu : BaseFormater {
        public byte Status;
    }

    public partial class Msg_C2G_LoadingProgress : BaseFormater {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        public byte Progress;
    }

    public partial class Msg_G2C_LoadingProgress : BaseFormater {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        public byte[] Progress;
    }

    public partial class Msg_G2C_AllFinishedLoaded : BaseFormater {
        public short Level;
    }

    public partial class Msg_C2G_GameEvent : BaseFormater {
        public byte[] Data;
    }

    public partial class Msg_G2C_GameEvent : BaseFormater {
        public byte[] Data;
    }

    #endregion
}