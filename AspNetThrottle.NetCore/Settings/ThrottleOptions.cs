using System;
using Microsoft.AspNetCore.Http;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Configuration options for throttle.
    /// </summary>
    public class ThrottleOptions : BasicOptions
    {
        internal Action<HttpContext, ClientRequest> ConfigureRequestAction { get; set; }

        /// <summary>
        /// Sets a action to configure <see cref="ClientRequest"/> object.
        /// </summary>
        /// <param name="action">The configuration action.</param>
        /// <returns>The option object.</returns>
        public ThrottleOptions ConfigureRequest(Action<HttpContext, ClientRequest> action)
        {
            ConfigureRequestAction = action;

            return this;
        }
    }
}
