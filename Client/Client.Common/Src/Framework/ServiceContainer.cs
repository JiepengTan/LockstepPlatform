using System.Collections.Generic;
using System.Linq;

namespace Lockstep.Game {
    public class ServiceContainer : IServiceContainer {
        private Dictionary<string, IService> _allServices = new Dictionary<string, IService>();

        public void RegisterService(IService service, bool overwriteExisting = true){
            var interfaceNames = service.GetType().FindInterfaces((type, criteria) =>
                    type.GetInterfaces()
                        .Any(t => t.FullName == typeof(IService).FullName), service)
                .Select(type => type.FullName).ToArray();

            foreach (var name in interfaceNames) {
                if (!_allServices.ContainsKey(name))
                    _allServices.Add(name, service);
                else if (overwriteExisting) {
                    _allServices[name] = service;
                }
            }
        }


        public T GetService<T>() where T : IService{
            var key = typeof(T).FullName;
            if (key == null) {
                return default(T);
            }

            if (!_allServices.ContainsKey(key)) {
                return default(T);
            }

            return (T) _allServices[key];
        }
    }
}