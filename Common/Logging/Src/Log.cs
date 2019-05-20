using System;

namespace Lockstep.Logging {
    public static class Log {
        public static LogSeverity LogSeverityLevel =
            LogSeverity.Info | LogSeverity.Warn | LogSeverity.Error | LogSeverity.Exception;

        public static event EventHandler<LogEventArgs> OnMessage;
        public static Action<bool, string> OnAssert;

        public static void SetLogAllSeverities(){
            LogSeverityLevel = LogSeverity.Trace | LogSeverity.Info | LogSeverity.Warn | LogSeverity.Error |
                               LogSeverity.Exception;
        }

        public static void Err(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Error, message, args);
        }

        public static void Warn(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Warn, message, args);
        }

        public static void Info(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Info, message, args);
        }

        public static void Trace(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Trace, message, args);
        }

        public static void Assert(object sender, bool val, string message){
            if (!val) {
                LogMessage(sender, LogSeverity.Error, "AssertFailed!!! " + message);
            }
        }

        private static void LogMessage(object sender, LogSeverity sev, string format, params object[] args){
            if (OnMessage != null && (LogSeverityLevel & sev) != 0) {
                var message = (args != null && args.Length > 0) ? string.Format(format, args) : format;
                OnMessage.Invoke(sender, new LogEventArgs(sev, message));
            }
        }
    }
}