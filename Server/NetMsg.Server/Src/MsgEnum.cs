namespace NetMsg.Server {
    //X daemon
    //Y daemonMaster
    //D database
    //I login
    //L lobby
    //S server
    //G gameServer
    //M master
    //W world
    //C client

    public enum EMsgMS:short {
        M2S_MasterToCandidate,
        S2M_RegisterServer,
        M2S_OnMasterServerChanged,
        M2S_BecomeCandidateServer,
        M2S_ReqShutdown,
        M2S_ReqShutdownAll,

        //OMS
        S2M_ReqOtherServerInfo,
        M2S_RepOtherServerInfo,
        EnumCount,
    }
    public enum EMsgYM:short {
        M2Y_RegisterServerInfo,
        Y2M_ReqOtherServerInfo,
        M2Y_RepOtherServerInfo,
        EnumCount,
    }

    public enum EMsgXS:short {
        S2X_ReqMasterInfo,
        X2S_RepMasterInfo,
        X2S_BorderMasterInfo,
        S2X_StartServer,
        S2X_ShutdownServer,
        S2X_ReqOtherServerInfo,
        EnumCount,
    }

    //X=MasterDaemon  D=Daemon
    public enum EMsgYX:short {
        X2Y_ReqMasterInfo,
        X2Y_ReqOtherServerInfo,
        M2Y_RepOtherServerInfo,
        Y2X_RepMasterInfo,
        Y2X_BorderMasterInfo,
        X2Y_RegisterDaemon,
        X2Y_ReportState,
        X2Y_ShutdownServer,

        EnumCount,
    }

    //D DB,S Server
    public enum EMsgDS:short {
        S2D_ReqUserInfo,
        D2S_RepUserInfo,
        S2D_ReqGameData,
        D2S_RepGameData,
        S2D_SaveGameData,
        D2S_SaveGameData,
        
        S2D_ReqCreateUser,
        D2S_RepCreateUser,
        S2D_RepChangeUserInfo,
        D2S_RepChangeUserInfo,
        
        S2D_SaveUserInfo,
        EnumCount,
    }
    //I login L lobby
    public enum EMsgLS:short {
        I2L_UserLogin,
        L2G_UserLeave,
        L2G_UserReconnect,
        L2G_CreateRoom,
        G2L_RegisterServer,
        G2L_OnStartGame,
        G2L_OnGameFinished,
        EnumCount,
    }

    // Server to Server
    public enum EMsgSS:short {
        I2L_UserLoginInfo,
        L2I_ReqUserLogin,
        EnumCount,
    }
}