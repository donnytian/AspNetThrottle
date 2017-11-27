using System;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetThrottle
{
    /// <summary>
    /// Memory cache implementation for <see cref="T:AspNetThrottle.ICounterStore" />.
    /// </summary>
    public class MemoryCacheCounterStore : ICounterStore
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheCounterStore"/> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache.</param>
        public MemoryCacheCounterStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public bool Exists(string id)
        {
            return _memoryCache.TryGetValue(id, out var _);
        }

        /// <inheritdoc />
        public RequestCounter Get(string id)
        {
            _memoryCache.TryGetValue(id, out RequestCounter value);

            return value;
        }

        /// <inheritdoc />
        public RequestCounter GetOrCreate(string id, Func<RequestCounter> createFunc, TimeSpan expirationTime)
        {
            var value = _memoryCache.GetOrCreate(id, entry =>
            {
                entry.SetAbsoluteExpiration(expirationTime);
                return createFunc();
            });

            return value;
        }

        /// <inheritdoc />
        public void Remove(string id)
        {
            _memoryCache.Remove(id);
        }

        /// <inheritdoc />
        public void Set(string id, RequestCounter value, TimeSpan expirationTime)
        {
            _memoryCache.Set(id, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expirationTime));
        }
    }
}
