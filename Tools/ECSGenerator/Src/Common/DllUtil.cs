using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using static Lockstep.Logger;

namespace Lockstep.ECGenerator.Common {
    public class DllUtil {
        public static Type[] LoadDll(string dllPath,Func<Type,bool> fileterFunc){
            if (!File.Exists(dllPath)) {
                LogError("Load dll failed  " + dllPath);
                return null;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            var types = assembly.GetTypes().Where(fileterFunc).ToArray();
            if (types.Length == 0) {
                LogError("dll do not have type of IGame :" + dllPath);
                return null;
            }

            return types;
        }
    }
}