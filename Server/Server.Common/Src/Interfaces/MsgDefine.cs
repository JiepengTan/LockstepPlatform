namespace Lockstep.Server.Common {
    public enum EServerMsg : byte {
        SlaveToMaster,
        MasterToSlave,
        MasterToCandidate,

        RegisterServer,
        OnMasterServerChanged,
        BecomeCandidateServer,
        ReqShutdown,
        ReqShutdownAll,
    }

    
    public enum EMasterType : byte {
        Master,
        CandidateMaster,
        Slave,
    }

    public enum EDaemonMsg : byte {
        RegisterServer,
        StartServer,
        ShutdownServer,
    }

    public enum EServerType {
        DaemonServer = 0,
        LoginServer = 1,
        LobbyServer = 2,
        GameServer = 3,
        DatabaseServer = 4,
        WorldServer =5,
    }
}