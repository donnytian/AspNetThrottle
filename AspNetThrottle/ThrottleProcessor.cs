using System;

namespace AspNetThrottle
{
    /// <summary>
    /// Represents the throttle processor for requests.
    /// </summary>
    public class ThrottleProcessor
    {
        /// <summary>
        /// Gets a <see cref="RequestCounter"/> that counted the current request according the specified rule.
        /// </summary>
        /// <param name="request">The client request object.</param>
        /// <param name="rule">The throttle rule.</param>
        /// <returns>The corresponding counter for the request.</returns>
        public virtual RequestCounter ProcessRequest(ClientRequest request, ThrottleRule rule)
        {
            throw new NotImplementedException();
        }
    }
}
