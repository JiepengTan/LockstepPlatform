using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lockstep.Client;
using Lockstep.Networking;
using Lockstep.Server.Common;
using NetMsg.Common;

namespace Test {
    internal class Program {

        static NetworkProxy GetDebugClient(){
             return new FakeClient();
        }
        public static void Main(string[] args){
            //TestNetwork();
            if (args.Length == 0) {
                ServerUtil.RunServerInThread(typeof(Lockstep.Server.Servers.Program).Assembly,EServerType.DaemonServer);
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            else {
               FakeClient. _RandomSeed = DateTime.Now.Millisecond;
            }

            ClientUtil.RunClient(GetDebugClient());
            while (true) {
                Thread.Sleep(30);
            }
        }
    }
}