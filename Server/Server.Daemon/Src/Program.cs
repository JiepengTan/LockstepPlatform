using Lockstep.Logging;
using Lockstep.Server.Common;
using Lockstep.Server.Daemon;
using Server.Common;

namespace Lockstep.Server.Daemon {
    class Program {
        static void Main(string[] args){
            var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.DaemonServer);
            ServerUtil.RunServer<DaemonServer>(config);
        }
    }
}