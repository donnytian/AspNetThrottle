using System;
using System.Collections.Generic;

namespace AspNetThrottle
{
    /// <summary>
    /// Configuration options for throttle.
    /// </summary>
    public class ThrottleOptions
    {
        /// <summary>
        /// Gets or sets the throttle name, used to compose the throttle cache keys.
        /// </summary>
        public string ThrottleName { get; set; }

        /// <summary>
        /// Gets or sets general rules for all clients.
        /// </summary>
        public List<ThrottleRule> GeneralRules { get; set; }

        /// <summary>
        /// Gets or sets client policies.
        /// </summary>
        public List<ClientPolicy> ClientPolicies { get; set; }

        /// <summary>
        /// Gets or sets the endpoint white list.
        /// </summary>
        public List<string> EndpointWhitelist { get; set; }

        /// <summary>
        /// Gets or sets the client whitelist.
        /// </summary>
        public List<string> ClientIdWhitelist { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Status code returned when quota exceeded, by default value is set to 429 (Too Many Requests).
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        /// <summary>
        /// Gets or sets a value that will be used as a formatter for the QuotaExceeded response message.
        /// If none specified the default will be:
        /// Resource calls quota exceeded! maximum admitted {0} per {1}
        /// </summary>
        public string QuotaExceededMessage { get; set; }
    }
}
