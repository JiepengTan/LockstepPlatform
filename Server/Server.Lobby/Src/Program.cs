using Lockstep.Server.Common;

namespace Lockstep.Server.Lobby {
    class Program {
        static void Main(string[] args){
            var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.LobbyServer);
            ServerUtil.RunServer<LobbyServer>(config);
        }
    }
}