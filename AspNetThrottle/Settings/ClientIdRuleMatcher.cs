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
        private readonly StringComparison _compareType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdRuleMatcher"/> class.
        /// </summary>
        /// <param name="idWhiltlist">Whitlist of client IDs.</param>
        /// <param name="clientPolicies">Client policies.</param>
        /// <param name="idIgnoreCase">Whether to ignore case when comparing ID.</param>
        public ClientIdRuleMatcher(IEnumerable<string> idWhiltlist, IEnumerable<ClientPolicy> clientPolicies, bool idIgnoreCase)
        {
            _whitelist = idWhiltlist;
            _clientPolicies = clientPolicies;
            _compareType = idIgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        }

        /// <inheritdoc />
        public bool IsWhitelisted(string clientId)
        {
            if (_whitelist == null || clientId.IsNullOrEmpty())
            {
                return false;
            }

            return _whitelist.Contains(clientId, _compareType);
        }

        /// <inheritdoc />
        public ThrottleRule[] GetClientRules(string clientId)
        {
            var list = new List<ThrottleRule>();

            if (clientId.IsNullOrWhiteSpace())
            {
                return list.ToArray();
            }

            var rules = _clientPolicies?.FirstOrDefault(p => clientId.Equals(p.ClientId, _compareType))?.Rules;

            if (rules != null)
            {
                list.AddRange(rules);
            }

            return list.ToArray();
        }
    }
}
