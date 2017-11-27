using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetThrottle.Tests.Fakes
{
    public class FakeCounterStore : ICounterStore
    {
        private readonly Dictionary<string, RequestCounter> _store = new Dictionary<string, RequestCounter>();

        public bool Exists(string id)
        {
            return _store.ContainsKey(id);
        }

        public RequestCounter Get(string id)
        {
            if (_store.ContainsKey(id))
            {
                return _store[id];
            }

            return null;
        }

        public RequestCounter GetOrCreate(string id, Func<RequestCounter> createFunc, TimeSpan expirationTime)
        {
            if (_store.ContainsKey(id))
            {
                return _store[id];
            }

            var counter = createFunc();
            _store[id] = counter;

            return counter;
        }

        public void Remove(string id)
        {
            if (_store.ContainsKey(id))
            {
                _store.Remove(id);
            }
        }

        public void Set(string id, RequestCounter value, TimeSpan expirationTime)
        {
            _store[id] = value;
        }
    }
}
