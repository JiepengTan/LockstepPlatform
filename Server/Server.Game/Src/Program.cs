using Lockstep.Logging;
using Lockstep.Server.Common;

namespace Lockstep.Server.Game {
    class Program {
        static void Main(string[] args){
            var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.GameServer);
            ServerUtil.RunServer<GameServer>(config);
        }
    }
}