namespace Lockstep.Server.Common  {
    public class Define {
        public const string ClientKey = "LockstepPlatform";
        public const string MSKey = "LockstepPlatform";
        public const string XSKey = "LockstepPlatform";
        public const int SimulationSpeed = 60;
    }
    
    
    public enum EMasterType : byte {
        Master,
        CandidateMaster,
        Slave,
        
        EnumCount,
    }
    public enum EServerType {
        Client=0,
        DaemonServer = 1,
        LoginServer = 2,
        LobbyServer = 3,
        GameServer = 4,
        DatabaseServer = 5,
        WorldServer = 6,
        
        EnumCount,
    }

}