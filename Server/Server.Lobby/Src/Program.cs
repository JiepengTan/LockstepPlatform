using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Lockstep.Logging;
using Lockstep.Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server {
    class Program {
        public class ConfigInfo {
            public string path;
            public int tcpPort;
            public int udpPort;
        }

        static ConfigInfo[] LoadConfigs(){
            List<ConfigInfo> allConfigs = new List<ConfigInfo>();
            var fileRelPath = "../Data/Config.json";
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileRelPath);
            string line = "";
            int i = 0;
            try {
                var allLine = File.ReadAllLines(dllPath);
                for (i = 0; i < allLine.Length; i++) {
                    line = allLine[i];
                    var elems = line.Split('=');
                    if (elems.Length != 3) {
                        Console.WriteLine($"Config Error: at lnie{i}:{line}");
                        continue;
                    }

                    int idx = 0;
                    var serverName = elems[idx++];
                    var tcpPort =int.Parse( elems[idx++]);
                    var udpPort = int.Parse(elems[idx++]);
                    allConfigs.Add(new ConfigInfo(){path =  serverName,tcpPort =  tcpPort,udpPort = udpPort});
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Config Error: at lnie{i}:{line} " + e);
            }
            return null;
        }

        static void OnLogMessage(object sernder, LogEventArgs logArgs){
            if ((LogSeverity.Error & logArgs.LogSeverity) != 0
                || (LogSeverity.Exception & logArgs.LogSeverity) != 0
            ) {
                StackTrace st = new StackTrace(true);
                StackFrame[] sf = st.GetFrames();
                for (int i = 4; i < sf.Length; ++i) {
                    var frame = sf[i];
                    sb.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" + frame.GetMethod().Name +
                                  " Line=" + frame.GetFileLineNumber());
                }
            }

            Console.WriteLine(logArgs.Message);
            if (sb.Length != 0) {
                Console.WriteLine(sb.ToString());
                sb.Length = 0;
                sb.Clear();
            }
        }

        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args){
            long lastTick = 1;
            int tickInterval = 40;
            Log.OnMessage += OnLogMessage;
            var assmeblies = AppDomain.CurrentDomain.GetAssemblies();
            var serverConfigs = LoadConfigs();
            List<BaseServer> servers = LauchServers(serverConfigs);

            var sw = new Stopwatch();
            Console.WriteLine("=============== LockstepServer Start!! ===============");
            sw.Start();
            foreach (var server in servers) {
                server.DoStart(9050, 9054);
                while (!Console.KeyAvailable) {
                    server.PollEvents();
                    var curTick = sw.ElapsedMilliseconds;

                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        Global.tickCount++;
                        Global.deltaTime = (int) elapse;
                        lastTick = curTick;
                        server.DoUpdate(Global.deltaTime);
                    }

                    Thread.Sleep(1);
                }
            }
        }

        private static List<BaseServer> LauchServers(ConfigInfo[] serverConfigs){
            List<BaseServer> servers = new List<BaseServer>();
            foreach (var config in serverConfigs) {
                var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.path);
                var assembly = Assembly.LoadFrom(dllPath);
                Debug.Log("Load dll " + dllPath);
                if (assembly != null) {
                    var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseServer))).ToArray();
                    var s = types.Length;
                    Debug.Assert(types.Length == 1);
                    var server = (BaseServer) System.Activator.CreateInstance(types[0], true);
                    servers.Add(server);
                }
            }

            return servers;
        }
    }
}