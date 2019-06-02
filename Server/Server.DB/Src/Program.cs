using Lockstep.Server.Common;

namespace Lockstep.Server.Database {
    internal class Program {
        static void Main(string[] args){
            var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.DatabaseServer);
            ServerUtil.RunServer<DatabaseServer>(config);
        }
    }
}