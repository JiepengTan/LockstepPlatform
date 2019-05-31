using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface IDaemonServer  {
        void RegisterServer(IGameServer server);
        void ReqStartServer(EServerType type);
        void StartServer(EServerType type);
        void ReportState(DaemonState state);
    }

    public class DaemonState : BaseFormater {
        public byte[] localServers;
        public float cpu;
        public float memory;
    }

}