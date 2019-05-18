namespace NetMsg.Game {
    //msg between client RoomServer
    public enum EMsgCS : byte {
        //Client GameServer
        C2S_ReqMissFrame,
        S2C_RepMissFrame,
        C2S_HashCode,
        C2S_PlayerInput,
        S2C_FrameData,
        C2S_GameEvent,
        S2C_GameEvent,
        
        C2S_PlayerReady,
        S2C_PlayerReady,
        C2S_PlayerLeave,
        S2C_PlayerLeave,
        S2C_StartGame,
        
        S2C_TickPlayer,
        EnumCount
    }

}