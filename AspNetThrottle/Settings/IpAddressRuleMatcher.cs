using System;
using System.Collections.Generic;
using System.Net;
using Extensions.String;

namespace AspNetThrottle
{
    /// <summary>
    /// An implementation of <see cref="IClientRuleMatcher"/> for IP address matching.
    /// </summary>
    public class IpAddressRuleMatcher : IClientRuleMatcher
    {
        private readonly IpSet _whitelist;
        private readonly Dictionary<IpSet, ClientPolicy> _ipPolicies = new Dictionary<IpSet, ClientPolicy>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressRuleMatcher"/> class.
        /// </summary>
        /// <param name="ipWhiltlist">Whitlist of IP addresses.</param>
        /// <param name="clientPolicies">Client policies.</param>
        public IpAddressRuleMatcher(IEnumerable<string> ipWhiltlist, IEnumerable<ClientPolicy> clientPolicies)
        {
            _whitelist = IpSet.ParseOrDefault(ipWhiltlist);

            if (clientPolicies != null)
            {
                foreach (var policy in clientPolicies)
                {
                    var ipSet = IpSet.ParseOrDefault(policy?.ClientId) ?? throw new ArgumentException($"Cannot parse to an IP set/ range from [{policy?.ClientId}]");
                    _ipPolicies[ipSet] = policy;
                }
            }
        }

        /// <inheritdoc />
        public bool IsWhitelisted(string ipAddress)
        {
            if (_whitelist == null || ipAddress.IsNullOrWhiteSpace())
            {
                return false;
            }

            return _whitelist.Contains(ipAddress);
        }

        /// <inheritdoc />
        public ThrottleRule[] GetClientRules(string ipAddress)
        {
            var list = new List<ThrottleRule>();

            if (ipAddress.IsNullOrWhiteSpace())
            {
                return list.ToArray();
            }

            foreach (var pair in _ipPolicies)
            {
                if (pair.Key.Contains(ipAddress))
                {
                    list.AddRange(pair.Value.Rules);
                }
            }

            return list.ToArray();
        }
    }
}
