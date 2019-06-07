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
    public static class NetworkUtil {
        public static bool InitNetServer<TMsgType, TParam>(ref NetServer<TMsgType, TParam> refServer, int port,
            object target)
            where TParam : class, INetProxy where TMsgType : struct{
            if (refServer != null) return true;
            var maxIdx = (short) (object) (TMsgType) Enum.Parse(typeof(TMsgType), "EnumCount");
            HashSet<string> tags = new HashSet<string>();
            for (short i = 0; i < maxIdx; i++) {
                var enumStr = ((TMsgType) (object) i).ToString();
                tags.Add(enumStr.Split('_')[0]);
            }

            refServer = new NetServer<TMsgType, TParam>(Define.MSKey, maxIdx, tags.ToArray(), target);
            refServer.Listen(port);
            return false;
        }


        public static bool InitNetClient<TMsgType>(ref NetClient<TMsgType> refClient, string ip, int port,
            Action onConnCallback, object target)
            where TMsgType : struct{
            if (refClient != null) return true;
            var maxIdx = (short) (object) (TMsgType) Enum.Parse(typeof(TMsgType), "EnumCount");
            HashSet<string> tags = new HashSet<string>();
            for (short i = 0; i < maxIdx; i++) {
                var enumStr = ((TMsgType) (object) i).ToString();
                tags.Add(enumStr.Split('_')[0]);
            }

            refClient = new NetClient<TMsgType>(maxIdx, tags.ToArray(), target);
            if (onConnCallback != null) {
                refClient.OnConnected += onConnCallback;
            }

            refClient.Connect(ip, port, Define.MSKey);
            return false;
        }
    }

    public static class ServerUtil {
        public const string defaultConfigPath = "../Data/Config.json";

        public static T CreateDelegateFromMethodInfo<T>(System.Object instance, MethodInfo method) where T : Delegate{
            return Delegate.CreateDelegate(typeof(T), instance, method) as T;
        }

        public static void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack, object obj)
            where TDelegate : Delegate
            where TEnum : struct{
            if (callBack == null) return;
            var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var method in methods) {
                var methodName = method.Name;
                if (methodName.StartsWith(prefix)) {
                    var eventTypeStr = methodName.Substring(ignorePrefixLen);
                    if (Enum.TryParse(eventTypeStr, out TEnum eType)) {
                        try {
                            var handler = CreateDelegateFromMethodInfo<TDelegate>(obj, method);
                            callBack(eType, handler);
                        }
                        catch (Exception e) {
                            Console.WriteLine("CreateDelegate failed " + eventTypeStr + " " + e);
                            throw;
                        }
                    }
                }
            }
        }

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

        public static void StartServices(){
            Time.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            Time.DoUpdate();
            CoroutineHelper.DoUpdate();
        }

        public static void RunServerInThread(Assembly Assembly, EServerType serverType){
            var thread = new Thread(() => {
                var config = ServerUtil.LoadConfig();
                ServerUtil.RunServer(Assembly, serverType, config);
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
            if (server == null) {
                Console.WriteLine("RunServer failed sobj is not a BaseServer");
                return;
            }

            var serverConfig = allConfig.GetServerConfig(serverType);
            long lastTick = 1;
            int tickInterval = 40;
            Console.WriteLine("=============== LockstepPlatform " + serverConfig.type + " Start!! ===============");
            Console.WriteLine("config: " + serverConfig.ToString());
            servers.Add(server);
            if (hasInited) {
                return;
            }

            hasInited = true;
            StartServices();

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
                        UpdateServices();
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