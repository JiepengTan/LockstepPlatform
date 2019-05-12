using System;
using System.Diagnostics;
using System.Threading;
using Lockstep.Logging;

namespace Lockstep.Logic.Server {
    class Program {
        static long lastTick = 1;
        static long deltaTime = 1;
        static int tickCount = 0;
        static int tickInterval = 40;

        static void Main(string[] args){
            Log.OnMessage += (sernder, logArgs) => { Console.WriteLine(logArgs.Message); };
            var sw = new Stopwatch();
            Console.WriteLine("=============== LockstepServer Start!! ===============");
            sw.Start();
            var server = new Lobby();
            server.DoStart(9050);
            while (!Console.KeyAvailable) {
                server.PollEvents();
                var curTick = sw.ElapsedMilliseconds;
                var elapse = curTick - lastTick;
                if (elapse >= tickInterval) {
                    Global.tickCount++;
                    Global.deltaTime = (int)elapse;
                    lastTick = curTick;
                    server.DoUpdate(Global.deltaTime);
                }
                Thread.Sleep(1);
            }
        }
    }
}