using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using LitJson;
using Lockstep.Util;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Common {
    public static class ServerUtil {
        public const string defaultConfigPath = "../../Data/Server/Config.json";

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

        private static List<BaseServer> servers = new List<BaseServer>();
        private static bool hasInited = false;


        public static void RunServerInThread(Assembly Assembly, EServerType serverType){
            var thread = new Thread(() => {
                var config = ServerUtil.LoadConfig();
                ServerUtil.RunServer(Assembly, serverType, config);
            });
            thread.Start();
        }
        public static void RunServerInThread(Assembly Assembly, BaseServer server){
            var thread = new Thread(() => {
                var config = ServerUtil.LoadConfig();
                ServerUtil.RunServer(Assembly, server, config);
            });
            thread.Start();
        }

        public static void RunServer(Assembly assembly, EServerType serverType, ConfigInfo allConfig){
            string serverTypeStr = "Lockstep.Server." + serverType.ToString().Replace("Server", "") + "." +
                                   (serverType).ToString();
            var type = assembly.GetType(serverTypeStr);
            if (type == null) {
                Console.WriteLine("StartServerFailed!: have no server type:" + serverTypeStr);
                return;
            }

            var sobj = Activator.CreateInstance(type);
            BaseServer server = sobj as BaseServer;
            server.serverType = serverType;
            RunServer(assembly, server,  allConfig);
        }

        public static void RunServer(Assembly assembly, BaseServer server,ConfigInfo allConfig){
            if (server == null) {
                Console.WriteLine("RunServer failed sobj is not a BaseServer");
                return;
            }

            var serverConfig = allConfig.GetServerConfig(server.serverType);
            long lastTick = 1;
            int tickInterval = 40;
            Console.WriteLine("=============== LockstepPlatform " + serverConfig.type + " Start!! ===============");
            Console.WriteLine("config: " + serverConfig.ToString());
            servers.Add(server);
            if (hasInited) {
                return;
            }

            hasInited = true;
            Utils.StartServices();

            var sw = new Stopwatch();
            sw.Start();
            {
                while (true) {
                    var count = servers.Count;
                    for (int i = 0; i < count; i++) {
                        var svr = servers[i];
                        if (!svr.HasInit) {
                            var initConfig =
                                allConfig.GetServerConfig((EServerType) (Enum.Parse(typeof(EServerType),
                                    svr.GetType().Name)));
                            svr.DoAwake(initConfig);
                            svr.DoStart();
                        }
                    }

                    foreach (var svr in servers) {
                        svr.PollEvents();
                    }

                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        lastTick = curTick;
                        Utils.UpdateServices();
                        foreach (var svr in servers) {
                            svr.DoUpdate((int) elapse);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
        }

        private static void RunServer(object sobj, ConfigInfo config){ }
    }
}