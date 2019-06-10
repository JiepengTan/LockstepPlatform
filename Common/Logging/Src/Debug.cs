using System;
using System.Diagnostics;

namespace Lockstep.Logging {
    public class Debug {
        public static string prefix = "";

        public static void Log(string format, params object[] args){
            Lockstep.Logging.Logger.Info(0, prefix + format, args);
        }

        public static void LogFormat(string format, params object[] args){
            Lockstep.Logging.Logger.Info(0, prefix + format, args);
        }

        public static void LogError(string format, params object[] args){
            Lockstep.Logging.Logger.Err(0, prefix + format, args);
        }

        public static void LogError(Exception e){
            Lockstep.Logging.Logger.Err(0, prefix + e.ToString());
        }

        public static void LogErrorFormat(string format, params object[] args){
            Lockstep.Logging.Logger.Err(0, prefix + format, args);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool val, string msg = ""){
            Lockstep.Logging.Logger.Assert(0, val, prefix+msg);
        }
    }
    
    public class DebugInstance {
        private string _prefix = "";

        public DebugInstance(string prefix){
            this._prefix = prefix;
        }

        public void SetPrefix(string prefix){
            _prefix = prefix;
        }

        public void Log(string format, params object[] args){
            Lockstep.Logging.Logger.Info(0, _prefix + format, args);
        }

        public void LogFormat(string format, params object[] args){
            Lockstep.Logging.Logger.Info(0, _prefix + format, args);
        }

        public void LogError(string format, params object[] args){
            Lockstep.Logging.Logger.Err(0, _prefix + format, args);
        }

        public void LogError(Exception e){
            Lockstep.Logging.Logger.Err(0, _prefix + e.ToString());
        }

        public void LogErrorFormat(string format, params object[] args){
            Lockstep.Logging.Logger.Err(0, _prefix + format, args);
        }

        [Conditional("DEBUG")]
        public void Assert(bool val, string msg = ""){
            Lockstep.Logging.Logger.Assert(0, val, _prefix+msg);
        }
    }
}