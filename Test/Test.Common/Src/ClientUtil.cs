using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lockstep.Networking;
using Lockstep.Util;

namespace Lockstep.Client {
 
    public class ClientUtil {
        private static List<NetworkProxy> clients = new List<NetworkProxy>();

        private static void StartServices(){
            Lockstep.Util.LTime.DoStart();
            CoroutineHelper.DoStart();
        }

        private static void UpdateServices(){
            Lockstep.Util.LTime.DoUpdate();
            CoroutineHelper.DoUpdate();
        }

        public static void RunClient(NetworkProxy loginManager){
            clients.Add(loginManager);
            Console.WriteLine(
                "=============== LockstepPlatform FakeClient" + clients.Count + " Start!! ===============");
            if (clients.Count == 1) {
                var thread = new Thread(_RunClient);
                thread.Start(true);
            }
        }

        private static void _RunClient(object isAloneObj){
            var isAlone = (bool) isAloneObj;
            long lastTick = 1;
            int tickInterval = 40;
            var sw = new Stopwatch();
            sw.Start();
            {
                while (true) {
                    var count = clients.Count;
                    for (int i = 0; i < count; i++) {
                        var svr = clients[i];
                        if (!svr.HasInit) {
                            svr.DoAwake();
                            svr.DoStart();
                        }
                    }

                    foreach (var svr in clients) {
                        svr.PollEvents();
                    }

                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        lastTick = curTick;
                        if (isAlone) {
                            UpdateServices();
                        }

                        foreach (var svr in clients) {
                            svr.DoUpdate((int) elapse);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
        }
    }
}