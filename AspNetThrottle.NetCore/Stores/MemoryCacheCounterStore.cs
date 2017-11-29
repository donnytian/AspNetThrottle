using System;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Memory cache implementation for <see cref="T:AspNetThrottle.ICounterStore" />.
    /// </summary>
    public class MemoryCacheCounterStore : ICounterStore
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheCounterStore"/> class.
        /// </summary>
        /// <param name="cache">The memory cache.</param>
        public MemoryCacheCounterStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public bool Exists(string id)
        {
            return _cache.TryGetValue(id, out var _);
        }

        /// <inheritdoc />
        public RequestCounter Get(string id)
        {
            _cache.TryGetValue(id, out RequestCounter value);

            return value;
        }

        /// <inheritdoc />
        public RequestCounter GetOrCreate(string id, Func<RequestCounter> createFunc, TimeSpan expirationTime)
        {
            var value = _cache.GetOrCreate(id, entry =>
            {
                entry.SetAbsoluteExpiration(expirationTime);
                return createFunc();
            });

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
            _cache.Set(id, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expirationTime));
        }
    }
}
