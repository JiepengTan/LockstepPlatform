using System;
using System.Threading;
using Lockstep.Client;
using Lockstep.Networking;

namespace Test.Tank {
    internal class Program {

        static NetworkProxy GetFakeUnityClient(){
            return new FakeUnityClient();
        }
        public static void Main(string[] args){
            //TestNetwork();
            if (args.Length == 0) {
                //ServerUtil.RunServerInThread(typeof(Lockstep.Server.Servers.Program).Assembly,EServerType.DaemonServer);
                //Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            else {
            }

            //ClientUtil.RunClient(GetDebugClient());
            ClientUtil.RunClient(GetFakeUnityClient());
            while (true) {
                Thread.Sleep(30);
            }
        }
    }
}