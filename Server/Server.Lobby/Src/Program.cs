using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Lockstep.Logging;

namespace Lockstep.Logic.Server {
    class Program {
        static long lastTick = 1;
        static int tickInterval = 40;
        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args){
            Log.OnMessage += (sernder, logArgs) => {
                if ((LogSeverity.Error & logArgs.LogSeverity) != 0
                    ||(LogSeverity.Exception & logArgs.LogSeverity) != 0
                    ) {
                    StackTrace st = new StackTrace(true);
                    StackFrame[] sf = st.GetFrames();
                    for (int i = 4; i < sf.Length; ++i) {
                        var frame = sf[i];
                        sb.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" +frame.GetMethod().Name + 
                                      " Line=" + frame.GetFileLineNumber());
                    }
                }
                Console.WriteLine(logArgs.Message);
                if (sb.Length != 0) {
                    Console.WriteLine(sb.ToString());
                    sb.Length = 0;
                    sb.Clear();
                }
            };
            var sw = new Stopwatch();
            Console.WriteLine("=============== LockstepServer Start!! ===============");
            sw.Start();
            var server = new Lobby();
            server.DoStart(9050,9054);
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