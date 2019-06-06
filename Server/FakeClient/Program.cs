using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lockstep.Util;

namespace Lockstep.FakeClient {
    internal class Program {
        public static List<Client> clients = new List<Client>();
        public static void StartServices(){
            Time.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            Time.DoUpdate();
            CoroutineHelper.DoUpdate();
        }
        public static void Main(string[] args){
            long lastTick = 1;
            int tickInterval = 40;
            Console.WriteLine("=============== LockstepPlatform FakeClient Start!! ===============");

            clients.Add(new Client());
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
                        UpdateServices();
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