namespace NetMsg.Lobby 
{
    //msg between client RoomServer
    public enum EMsgCL : byte
    {   
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
        
        C2L_RoomMsg,
        L2C_RoomMsg,
        L2C_RoomStatuUpdate,
        L2C_LobbyStatuUpdate,
        EnumCount,
    }
}