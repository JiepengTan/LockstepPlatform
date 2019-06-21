using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVGenCode {
    public class Debug {
        static public void LogError(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
#else
            Console.WriteLine(message);
#endif
        }
        static public void Log(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }
    }
}
