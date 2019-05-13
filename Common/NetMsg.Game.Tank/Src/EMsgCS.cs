using NetMsg.Lobby;

namespace NetMsg.Game.Tank
{
    //msg between client RoomServer
    public enum EMsgCS : byte
    {   
        //Client GameServer
        C2S_PlayerInput,
        S2C_StartGame ,
        S2C_FrameData ,
        C2S_ReqMissPack,
        S2C_RepMissPack,
        C2S_HashCode,
        EnumCount
    }
}