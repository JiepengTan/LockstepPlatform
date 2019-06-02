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

    public enum EMsgMS:short {
        M2S_MasterToCandidate,
        S2M_RegisterServer,
        M2S_OnMasterServerChanged,
        M2S_BecomeCandidateServer,
        M2S_ReqShutdown,
        M2S_ReqShutdownAll,

        EnumCount,
    }


    public enum EMsgXS:short {
        S2X_ReqMasterInfo,
        X2S_RepMasterInfo,
        S2X_StartServer,
        S2X_ShutdownServer,

        EnumCount,
    }

    //X=MasterDaemon  D=Daemon
    public enum EMsgYX:short {
        X2Y_ReqMasterInfo,
        Y2X_RepMasterInfo,
        X2Y_RegisterDaemon,
        X2Y_ReportState,
        X2Y_ShutdownServer,

        EnumCount,
    }

    //D DB,S Server
    public enum EMsgDS:short {
        S2D_ReqUserInfo,
        D2S_RepUserInfo,
        S2D_ReqCreateUser,
        D2S_RepCreateUser,
        S2D_RepChangeUserInfo,
        D2S_RepChangeUserInfo,

        EnumCount,
    }

    // Server to Server
    public enum EMsgSS:short {
        I2L_UserLoginInfo,
        L2I_ReqUserLogin,
        EnumCount,
    }
}