using AspNetThrottle;
using AspNetThrottle.NetCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Helper functions for configuring identity services.
    /// </summary>
    public class ThrottleBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleBuilder" /> class.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to attach to.</param>
        public ThrottleBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection" /> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection" /> services are attached to.
        /// </value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Adds a <see cref="MemoryCacheCounterStore" /> for the <seealso cref="ICounterStore" />.
        /// </summary>
        /// <returns>The current <see cref="ThrottleBuilder" /> instance.</returns>
        public virtual ThrottleBuilder AddMemoryCacheCounterStore()
        {
            Services.AddMemoryCache();
            Services.AddSingleton<ICounterStore, MemoryCacheCounterStore>();

            return this;
        }

        /// <summary>
        /// Adds a <see cref="DistributedCacheCounterStore" /> for the <seealso cref="ICounterStore" />.
        /// If there is no distributed cache implementations added (like redis or sql server), a default in-memory cache will be added.
        /// </summary>
        /// <returns>The current <see cref="ThrottleBuilder" /> instance.</returns>
        public virtual ThrottleBuilder AddDistributedCacheCounterStore()
        {
            var distributedService = Services.BuildServiceProvider().GetService<IDistributedCache>();

            if (distributedService == null)
            {
                Services.AddDistributedMemoryCache();
            }

            Services.AddSingleton<ICounterStore, DistributedCacheCounterStore>();

            return this;
        }
    }
}
