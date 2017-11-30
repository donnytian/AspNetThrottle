using System;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Distributed cache implementation for <see cref="T:AspNetThrottle.ICounterStore" />.
    /// </summary>
    public class DistributedCacheCounterStore : ICounterStore
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheCounterStore"/> class.
        /// </summary>
        /// <param name="cache">The distributed cache.</param>
        public DistributedCacheCounterStore(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <inheritdoc />
        public bool Exists(string id)
        {
            var s = _cache.GetString(id);
            return !string.IsNullOrEmpty(s);
        }

        /// <inheritdoc />
        public RequestCounter Get(string id)
        {
            var s = _cache.GetString(id);

            if (!string.IsNullOrEmpty(s))
            {
                return JsonConvert.DeserializeObject<RequestCounter>(s);
            }

            return null;
        }

        /// <inheritdoc />
        public RequestCounter GetOrCreate(string id, Func<RequestCounter> createFunc, TimeSpan expirationTime)
        {
            var s = _cache.GetString(id);

            if (!string.IsNullOrEmpty(s))
            {
                return JsonConvert.DeserializeObject<RequestCounter>(s);
            }

            var value = createFunc();
            Set(id, value, expirationTime);

            return value;
        }

        /// <inheritdoc />
        public void Remove(string id)
        {
            _cache.Remove(id);
        }

        /// <inheritdoc />
        public void Set(string id, RequestCounter value, TimeSpan expirationTime)
        {
            _cache.SetString(id, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime));
        }

        /// <inheritdoc />
        public void Set(string id, RequestCounter value, DateTimeOffset absolute)
        {
            _cache.SetString(id, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions().SetAbsoluteExpiration(absolute));
        }
    }
}
