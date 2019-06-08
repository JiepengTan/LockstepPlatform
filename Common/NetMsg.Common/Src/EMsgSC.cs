
namespace NetMsg.Common {

    public enum EServerDetailPortType {
        ServerPort,
        TcpPort,
        UdpPort,
    }

    public enum ELoginResult {
        Succ,
        PasswordMissMatch,
        ErrorHash,
        NotLogin,
    }

    public enum ERoomOperatorResult {
        Succ,
        Full,
        NotExist,
        AlreadyExist,
    }

  
    
    //msg between client RoomServer
    public enum EMsgSC : short {
        S2C_TickPlayer,

        //IC
        C2I_UserLogin,
        I2C_LoginResult,

        //LC
        //chat
        C2L_RoomChatInfo,
        L2C_RoomChatInfo,
        
        //Room Operator
        I2L_UserLogin,
        C2L_UserLogin,
        //Room Operator
        C2L_ReqRoomList,
        L2C_RoomList,
        L2C_RoomInfoUpdate,
        
        C2L_JoinRoom,
        L2C_JoinRoom,
        L2C_JoinRoomResult,
        C2L_ReadyInRoom,
        L2C_ReadyInRoom,
        C2L_LeaveRoom,
        L2C_LeaveRoom,
        C2L_CreateRoom,
        L2C_CreateRoomResult,
        C2L_StartGame,
        L2C_StartGame,

        //LG
        L2G_StartGame,

        //GC Tcp
        C2G_Hello,
        G2C_Hello,
        G2C_GameInfo,

        G2C_GameStatu,
        C2G_LoadingProgress,
        G2C_LoadingProgress,
        G2C_AllFinishedLoaded,

        //
        C2G_GameEvent,
        G2C_GameEvent,

        //GC udp
        C2G_UdpHello,
        C2G_ReqMissFrame,
        C2G_RepMissFrameAck,
        G2C_RepMissFrame,
        C2G_HashCode,
        C2G_PlayerInput,
        G2C_FrameData,

        EnumCount
    }
}