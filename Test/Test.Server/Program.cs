using System;
using System.Threading;
using Lockstep.Client;
using Lockstep.Networking;
using Lockstep.Server.Common;

namespace Test {
    internal class Program {

        static NetworkProxy GetDebugClient(){
             return new FakeClient();
        }

        private static NetworkProxy client;
        public static void Main(string[] args){
            //TestNetwork();
            if (args.Length == 0) {
                ServerUtil.RunServerInThread(typeof(Lockstep.Server.Servers.Program).Assembly,EServerType.DaemonServer);
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            else {
               FakeClient. _RandomSeed = DateTime.Now.Millisecond;
            }

            client = GetDebugClient();
            ClientUtil.RunClient(client);
            while (true) {
                var cmd = Console.ReadLine();
                if (cmd == "kc") {
                    client.DoDestroy();
                }

                if (cmd == "nc") {
                    client = GetDebugClient();
                    ClientUtil.RunClient(client);
                }
                Thread.Sleep(30);
            }
        }
    }
}