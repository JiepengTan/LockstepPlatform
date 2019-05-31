using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LitJson;
using Lockstep.Server.Common;

namespace Server.Common {
   
    public static class ServerUtil {
        public const string defaultConfigPath = "../Data/Config.json";

        public static ConfigInfo LoadConfig(string relPath = defaultConfigPath){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relPath);
            var json = File.ReadAllText(path);
            ConfigInfo config = null;
            try {
                config = JsonMapper.ToObject<ConfigInfo>(json);
            }
            catch (Exception e) {
                Console.WriteLine("LoadConfig failed!" + path + e);
            }

            return config;
        }
        public static void RunServer<T>(ServerConfigInfo config)
            where T : BaseServer, new(){
            long lastTick = 1;
            int tickInterval = 40;
            Console.WriteLine("=============== LockstepPlatform " + config.type + " Start!! ===============");
            Console.WriteLine("config: " + config.ToString());
            var sw = new Stopwatch();
            sw.Start();
            BaseServer server = new T();
            {
                server.DoStart(config);
                while (!Console.KeyAvailable) {
                    server.PollEvents();
                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        lastTick = curTick;
                        server.DoUpdate((int) elapse);
                    }

                    Thread.Sleep(1);
                }
            }
        }
    }
}