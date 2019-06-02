using System;
using System.Diagnostics;

namespace Lockstep.Logging {
    public class Debug {
        public static string prefix = "";

        public static void Log(string format, params object[] args){
            Lockstep.Logging.Log.Info(0, prefix + format, args);
        }

        public static void LogFormat(string format, params object[] args){
            Lockstep.Logging.Log.Info(0, prefix + format, args);
        }

        public static void LogError(string format, params object[] args){
            Lockstep.Logging.Log.Err(0, prefix + format, args);
        }

        public static void LogError(Exception e){
            Lockstep.Logging.Log.Err(0, prefix + e.ToString());
        }

        public static void LogErrorFormat(string format, params object[] args){
            Lockstep.Logging.Log.Err(0, prefix + format, args);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool val, string msg = ""){
            Lockstep.Logging.Log.Assert(0, val, prefix+msg);
        }
    }
}