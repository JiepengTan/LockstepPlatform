using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Lockstep.ECGenerator {
    public class DllUtil {
        public static Type[] LoadDll(string dllPath,Func<Type,bool> fileterFunc){
            if (!File.Exists(dllPath)) {
                Lockstep.Logging.Debug.LogError("Load dll failed  " + dllPath);
                return null;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            var types = assembly.GetTypes().Where(fileterFunc).ToArray();
            if (types.Length == 0) {
                Lockstep.Logging.Debug.LogError("dll do not have type of IGame :" + dllPath);
                return null;
            }

            return types;
        }
    }
}