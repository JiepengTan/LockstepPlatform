using System.Net;

namespace Lockstep.Server.Common {
    public interface IServerProxy {
        IPEndPoint ipInfo { get; set; }
        void SendMsg(byte[] data);
    }

    public class ServerProxy : IServerProxy {
        public IPEndPoint ipInfo { get; set; }
        public void SendMsg(byte[] data){ }
    }
}