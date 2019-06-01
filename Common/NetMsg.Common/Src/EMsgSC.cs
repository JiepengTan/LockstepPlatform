namespace NetMsg.Common
{
    //msg between client RoomServer
    public enum EMsgSC : byte
    {   
        //Login
        S2C_TickPlayer,
        
        C2I_Register,
        I2C_RegisterResult,
        C2I_ReqLogin,
        I2C_LoginResult,
        C2I_ChangeUserInfo,
        I2C_ChangeUserInfoResult,
        
        //Lobby
        C2L_ReqLogin,
        L2C_RepLogin,
        C2L_ReqRoomList,
        L2C_RepRoomList,
        
        C2L_JoinRoom,
        L2C_JoinRoom,
        C2L_CreateRoom,
        L2C_CreateRoom,
        C2L_LeaveRoom,
        L2C_LeaveRoom,
        C2L_PlayerReady,
        L2C_PlayerReady,
        C2L_StartGame,
        L2C_StartGame,
        
        L2C_RoomStatuUpdate,
        L2C_LobbyStatuUpdate,
        
        //Game
        //Input udp
        C2G_ReqMissFrame,
        C2G_RepMissFrameAck,
        G2C_RepMissFrame,
        C2G_HashCode,
        C2G_PlayerInput,
        G2C_FrameData,

        //Level
        G2C_LoadingProgress,
        C2G_LoadingProgress,
        C2G_PartFinished,
        G2C_PartFinished,
        G2C_StartGame,

        //GameEvent tcp
        C2G_GameEvent,
        G2C_GameEvent,
        
        EnumCount
    }
}