namespace NetMsg.Lobby 
{
    //msg between client RoomServer
    public enum EMsgCL : byte
    {   
        //Client Lobby
        C2L_InitMsg,
        L2C_ReqInit,
        C2L_JoinRoom,
        C2L_CreateRoom,
        C2L_LeaveRoom,
        C2L_RoomMsg,
        L2C_RoomMsg,
        L2C_JoinRoomResult,
        C2L_PlayerReady,
        EnumCount
    }
}