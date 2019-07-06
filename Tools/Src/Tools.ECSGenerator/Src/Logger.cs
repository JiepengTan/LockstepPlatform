using System;

namespace Lockstep {
    public class Logger {
        public static void LogError(string format, params object[] pa){
            Console.WriteLine(string.Format(format, pa));
        }
    }
}