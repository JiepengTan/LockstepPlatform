using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using LitJson;
using Lockstep.Server.Common;
using Lockstep.Util;

namespace Server.Common {
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
                                                   BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                var methodName = method.Name;
                if (methodName.StartsWith(prefix)) {
                    var eventTypeStr = methodName.Substring(ignorePrefixLen);
                    if (Enum.TryParse(eventTypeStr, out TEnum eType)) {
                        var handler = CreateDelegateFromMethodInfo<TDelegate>(obj, method);
                        callBack(eType, handler);
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

        public static void StartServices(){
            Time.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            Time.DoUpdate();
            CoroutineHelper.DoUpdate();
        }

        public static void RunServer<T>(ServerConfigInfo config)
            where T : BaseServer, new(){
            long lastTick = 1;
            int tickInterval = 40;
            Console.WriteLine("=============== LockstepPlatform " + config.type + " Start!! ===============");
            Console.WriteLine("config: " + config.ToString());
            var sw = new Stopwatch();
            sw.Start();
            StartServices();
            BaseServer server = new T();
            {
                server.DoAwake(config);
                server.DoStart(config);
                while (!Console.KeyAvailable) {
                    server.PollEvents();
                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        lastTick = curTick;
                        UpdateServices();
                        server.DoUpdate((int) elapse);
                    }

                    Thread.Sleep(1);
                }
            }
        }
    }
}