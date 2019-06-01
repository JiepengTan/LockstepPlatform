﻿using Lockstep.Server.Common;
using Server.Common;

namespace Lockstep.Server.Login {
    internal class Program {
        static void Main(string[] args){
            var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.LoginServer);
            ServerUtil.RunServer<LoginServer>(config);
        }
    }
}