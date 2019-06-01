namespace Lockstep.Server.Common  {
    public class Define {
        public const string ClientKey = "LockstepPlatform";
        public const string MSKey = "LockstepPlatformMasterSlave";
        public const string XSKey = "LockstepPlatformDaemonServer";
        public const int SimulationSpeed = 60;
    }
    
    
    public enum EMasterType : byte {
        Master,
        CandidateMaster,
        Slave,
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