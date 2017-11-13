using System;
using System.Collections.Generic;

namespace AspNetThrottle
{
    /// <summary>
    /// Configuration policy for a specific client.
    /// </summary>
    public class ClientPolicy
    {
        /// <summary>
        /// Gets or sets the client identity.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the rule collection.
        /// </summary>
        public List<ThrottleRule> Rules { get; set; }
    }
}
