﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using Lockstep.Server.Common;
using Lockstep.Server.Daemon;
using Lockstep.Server.Login;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Servers {


    public class Program {
        static void Main(string[] args){
            EServerType serverType = EServerType.DaemonServer;
            if (args.Length > 0) {
                if (Enum.TryParse<EServerType>(args[0], out var type)) {
                    serverType = type;
                }
                else {
                    Console.WriteLine("StartServer failed! Unknown server type " + args[0]);
                    return;
                }
            }
            ServerUtil.RunServerInThread(typeof(Program).Assembly, serverType);
        }
    }
}