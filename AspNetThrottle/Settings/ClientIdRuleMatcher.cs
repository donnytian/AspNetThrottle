using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.String;

namespace AspNetThrottle
{
    /// <summary>
    /// An implementation of <see cref="IClientRuleMatcher"/> for client ID matching.
    /// </summary>
    public class ClientIdRuleMatcher : IClientRuleMatcher
    {
        private readonly IEnumerable<string> _whitelist;
        private readonly IEnumerable<ClientPolicy> _clientPolicies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdRuleMatcher"/> class.
        /// </summary>
        /// <param name="idWhiltlist">Whitlist of client IDs.</param>
        /// <param name="clientPolicies">Client policies.</param>
        public ClientIdRuleMatcher(IEnumerable<string> idWhiltlist, IEnumerable<ClientPolicy> clientPolicies)
        {
            _whitelist = idWhiltlist ?? throw new ArgumentNullException(nameof(idWhiltlist));
            _clientPolicies = clientPolicies ?? throw new ArgumentNullException(nameof(clientPolicies));
        }

        /// <inheritdoc />
        public bool IsWhitelisted(string clientId)
        {
            return _whitelist.Contains(clientId);
        }

        /// <inheritdoc />
        public ThrottleRule[] GetClientRules(string clientId)
        {
            var list = new List<ThrottleRule>();

            if (clientId.IsNullOrWhiteSpace())
            {
                return list.ToArray();
            }

            var rules = _clientPolicies.FirstOrDefault(p => p.ClientId == clientId)?.Rules;

            if (rules != null)
            {
                list.AddRange(rules);
            }

            return list.ToArray();
        }
    }
}
