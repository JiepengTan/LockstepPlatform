using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Logging;

namespace Lockstep.Server.Database
{
    public class DbAccessorFactory 
    {
        private Dictionary<Type, object> _accessors = new Dictionary<Type, object>();
        
        /// Adds a database accessor to the list of available accessors
        public void SetAccessor<T>(object access)
        {
            if (_accessors.ContainsKey(typeof(T)))
            {
                Debug.LogError(string.Format("Database accessor of type {0} was overwriten", typeof(T)));
            }

            _accessors[typeof(T)] = access;
        }

        /// Retrieves a database accessor from a list of available accessors
        public T GetAccessor<T>() where T : class
        {
            object result;
            _accessors.TryGetValue(typeof(T), out result);

            return result as T;
        }
    }
}