namespace NetMsg.Server {
    
    //X daemon
    //Y daemonMaster
    //D database
    //I login
    //L lobby
    //S server
    //M master
    //W world
    //C client
    
    public enum EMsgMS : byte {
        SlaveToMaster,
        MasterToSlave,
        MasterToCandidate,

        RegisterServer,
        OnMasterServerChanged,
        BecomeCandidateServer,
        ReqShutdown,
        ReqShutdownAll,
        
        EnumCount,
    }
    
    
    public enum EMsgXS : byte {
        S2X_RegisterServer,
        S2X_StartServer,
        S2X_ShutdownServer,
        S2X_ReqMasterInfo,
        X2S_RepMasterInfo,
        
        EnumCount,
    }
    //X=MasterDaemon  D=Daemon
    public enum EMsgYX : byte {
        X2Y_RegisterDaemon,
        X2Y_ReportState,
        X2Y_ShutdownServer,
        
        EnumCount,
    }
    
    //D DB,S Server
    public enum EMsgDS{
        S2D_ReqUserInfo,
        D2S_RepUserInfo,
        S2D_ReqCreateUser,
        D2S_RepCreateUser,
        S2D_RepChangeUserInfo,
        D2S_RepChangeUserInfo,
        
        EnumCount,
    }
    // Server to Server
    public enum EMsgSS {
        I2L_UserLoginInfo,
        L2I_ReqUserLogin,
        EnumCount,
    }
    
}