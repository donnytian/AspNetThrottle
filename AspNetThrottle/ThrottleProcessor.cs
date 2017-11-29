using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.String;
using Extensions.String.Cryptography;

namespace AspNetThrottle
{
    /// <summary>
    /// Provide core functions for request throttle.
    /// </summary>
    public class ThrottleProcessor
    {
        private readonly ICounterStore _counterStore;
        private readonly IClientRuleMatcher _ruleMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleProcessor"/> class.
        /// </summary>
        /// <param name="options">The option object.</param>
        /// <param name="counterStore">The store for throttle data.</param>
        /// <param name="ruleMatcher">The matcher to get matching rules for specific client.</param>
        public ThrottleProcessor(BasicOptions options, ICounterStore counterStore, IClientRuleMatcher ruleMatcher)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _counterStore = counterStore ?? throw new ArgumentNullException(nameof(counterStore));
            _ruleMatcher = ruleMatcher ?? throw new ArgumentNullException(nameof(ruleMatcher));
        }

        /// <summary>
        /// Gets throttle options.
        /// </summary>
        public BasicOptions Options { get; }

        /// <summary>
        /// Gets a <see cref="RequestCounter"/> that counted the current request according the specified rule.
        /// </summary>
        /// <param name="request">The client request object.</param>
        /// <param name="rule">The throttle rule.</param>
        /// <returns>The corresponding counter for the request.</returns>
        public RequestCounter ProcessRequest(ClientRequest request, ThrottleRule rule)
        {
            if (rule.PeriodTimespan == null)
            {
                throw new InvalidOperationException("Throttle period time span is not set!");
            }

            var key = GetStorageKey(request, rule);
            var counter = _counterStore.GetOrCreate(
                key,
                () => new RequestCounter
                {
                    Timestamp = DateTime.UtcNow,
                    TotalRequests = 0
                },
                rule.PeriodTimespan.Value);

            counter.Increment();

            // Checks limit and enters cool down period by prolonging the counter expiration.
            if (counter.TotalRequests > rule.Limit && !counter.LimitExceeded)
            {
                counter.LimitExceeded = true;

                if (rule.CooldownTimespan.HasValue)
                {
                    _counterStore.Set(key, counter, rule.CooldownTimespan.Value);
                }
            }

            return counter;
        }

        /// <summary>
        /// Checks if the request is white-listed.
        /// </summary>
        /// <param name="request">The request object to be checked.</param>
        /// <returns>True if the request is white-listed; otherwise false.</returns>
        public bool IsWhitelisted(ClientRequest request)
        {
            if (_ruleMatcher.IsWhitelisted(request.ClientId))
            {
                return true;
            }

            if (Options.EndpointWhitelist != null)
            {
                if (Options.EndpointWhitelist.Any(x => $"{request.HttpVerb}:{request.Path}".StartsWith(x, StringComparison.CurrentCultureIgnoreCase)) ||
                    Options.EndpointWhitelist.Any(x => $"*:{request.Path}".StartsWith(x, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets matching rules for the request.
        /// If any rule in client policy is matched, all general rules will be ignored; otherwise general rules will be checked.
        /// </summary>
        /// <param name="request">The request to be matched.</param>
        /// <returns>The array of rules that should apply on specified request.</returns>
        public ThrottleRule[] GetMatchingRules(ClientRequest request)
        {
            var clientRules = _ruleMatcher.GetClientRules(request.ClientId);

            var matchingPolicyRules = FilterRulesByEndpoint(request, clientRules);

            if (matchingPolicyRules.Any())
            {
                return matchingPolicyRules.ToArray();
            }

            return FilterRulesByEndpoint(request, Options.GeneralRules).ToArray();
        }

        private static List<ThrottleRule> FilterRulesByEndpoint(ClientRequest request, IEnumerable<ThrottleRule> rules)
        {
            var filtered = new List<ThrottleRule>();

            if (rules == null)
            {
                return filtered;
            }

            var req = $"{request.HttpVerb}:{request.Path}".ToLowerInvariant();
            foreach (var rule in rules)
            {
                if (rule.Endpoint.IsNullOrWhiteSpace() || req.IsMatch(rule.Endpoint.ToLowerInvariant()))
                {
                    filtered.Add(rule);

                    if (rule.StopProbingWhenMatched)
                    {
                        break;
                    }
                }
            }

            return filtered;
        }

        private string GetStorageKey(ClientRequest request, ThrottleRule rule)
        {
            return $"{Options.ThrottleName}_{request.ClientId}_{rule.Id}".Sha1();
        }
    }
}
