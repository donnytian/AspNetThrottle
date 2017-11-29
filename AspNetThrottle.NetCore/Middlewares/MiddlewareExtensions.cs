using System;
using AspNetThrottle.NetCore;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extensions for throttle middlewares.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>Adds services required for using throttles.</summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <returns>The <see cref="ThrottleBuilder" /> so that additional calls can be chained.</returns>
        public static ThrottleBuilder AddThrottle(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new ThrottleBuilder(services);
        }

        /// <summary>
        /// Adds the <see cref="IpThrottleMiddleware" /> to the specified <see cref="IApplicationBuilder" />, which enables throttle capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
        /// <param name="options">The throttle options.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseIpThrottle(this IApplicationBuilder app, ThrottleOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<IpThrottleMiddleware>(options);
        }

        /// <summary>
        /// Adds the <see cref="IdThrottleMiddleware" /> to the specified <see cref="IApplicationBuilder" />, which enables throttle capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
        /// <param name="options">The throttle options.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseIdThrottle(this IApplicationBuilder app, ThrottleOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<IdThrottleMiddleware>(options);
        }
    }
}
