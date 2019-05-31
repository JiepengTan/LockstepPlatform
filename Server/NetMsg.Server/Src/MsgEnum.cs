namespace NetMsg.Server {

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
    
    
    public enum EMsgDS : byte {
        S2D_RegisterServer,
        S2D_StartServer,
        S2D_ShutdownServer,
        S2D_ReqMasterInfo,
        D2S_RepMasterInfo,
        EnumCount,
    }
    //X=MasterDaemon  D=Daemon
    public enum EMsgXD : byte {
        D2X_RegisterDaemon,
        D2X_ReportState,
        D2X_ShutdownServer,
        EnumCount,
    }
    
}