namespace Lockstep.NetMsg.Lobby 
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
        
        //Client GameServer
        C2S_PlayerInput,
        S2C_StartGame,
        S2C_FrameData,
        C2S_ReqMissPack,
        S2C_RepMissPack,
        C2S_HashCode,
        
        EnumCount
    }
}