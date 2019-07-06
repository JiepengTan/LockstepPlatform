using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.AccessControl;
using Lockstep.Server.Common;
using Lockstep.Server.Daemon;
using Lockstep.Server.Database;
using Lockstep.Server.Game;
using Lockstep.Server.Lobby;
using Lockstep.Server.Login;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Servers {


    public class Program {
        internal static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args) {
            string libPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                             Path.DirectorySeparatorChar;
            var path = libPath + args.Name.Split(',')[0] + ".dll";
            var assembly = Assembly.LoadFrom(path);
            return assembly;
        }
 
        static void Main(string[] args){
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(dir);
            foreach (var path in files) {
                if (path.EndsWith(".dll")) {
                   var ass =  Assembly.LoadFrom(path);
                   ass = null;
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            AssemblyLoadContext.Default.Resolving+= (a, name) => {
                string libPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                 Path.DirectorySeparatorChar;
                var path = libPath + name.Name.Split(',')[0] + ".dll";
                return Assembly.LoadFrom(path);

            };
            var assembly =  typeof (Program).GetTypeInfo().Assembly;
            var exAssembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            SubMain(args);
        }
        static void SubMain(string[] args){
            EServerType serverType = EServerType.DaemonServer;
            if (args.Length > 0) {
                if (Enum.TryParse<EServerType>(args[0], out var type)) {
                    serverType = type;
                }
                else {
                    Console.WriteLine("StartServer failed! Unknown server type " + args[0]);
                    return;
                }
            }

            ServerUtil.RunServerInThread(Assembly.GetExecutingAssembly(), serverType);
        }
    }
}