
namespace Lockstep.Logging {
    public class Debug {
        public static void Log(string format, params object[] args){
            Lockstep.Logging.Log.Info(0, format, args);
        }
        public static void LogFormat(string format, params object[] args){
            Lockstep.Logging.Log.Info(0, format, args);
        }
        public static void LogError(string format, params object[] args){
            Lockstep.Logging.Log.Err(0, format, args);
        }

        public static void LogErrorFormat(string format, params object[] args){
            Lockstep.Logging.Log.Err(0, format, args);
        }
        
        [Conditional("DEBUG")]
        public static void Assert(bool val, string msg = ""){
            Lockstep.Logging.Log.Assert(0, val, msg);
        }
    }
}