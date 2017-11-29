using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Represents an ASP.NET Core middleware to limit API access rate based on client ID.
    /// </summary>
    public class IdThrottleMiddleware : ThrottleMiddleware
    {
        public IdThrottleMiddleware(RequestDelegate next, ThrottleOptions options, ICounterStore counterStore, ILogger<IdThrottleMiddleware> logger)
            : base(next, options, counterStore, logger)
        {
            var matcher = new ClientIdRuleMatcher(Options.ClientWhitelist, Options.ClientPolicies);
            Processor = new ThrottleProcessor(Options, counterStore, matcher);
        }

        /// <inheritdoc />
        public override ClientRequest GetClientRequest(HttpContext context)
        {
            var request = new ClientRequest();

            if (Options.ConfigureRequestAction != null)
            {
                Options.ConfigureRequestAction(context, request);
            }
            else
            {
                request.ClientId = context.User?.Identity?.Name;
            }

            request.HttpVerb = request.HttpVerb ?? context.Request.Method.ToLower();
            request.Path = request.Path ?? context.Request.Path.ToString().ToLowerInvariant();

            return request;
        }
    }
}
