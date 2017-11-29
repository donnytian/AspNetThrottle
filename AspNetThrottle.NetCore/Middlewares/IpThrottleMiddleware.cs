using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Represents an ASP.NET Core middleware to limit API access rate based on IP address.
    /// </summary>
    public class IpThrottleMiddleware : ThrottleMiddleware
    {
        public IpThrottleMiddleware(RequestDelegate next, ThrottleOptions options, ICounterStore counterStore, ILogger<IdThrottleMiddleware> logger)
            : base(next, options, counterStore, logger)
        {
            var matcher = new IpAddressRuleMatcher(Options.ClientWhitelist, Options.ClientPolicies);
            Processor = new ThrottleProcessor(Options, counterStore, matcher);
        }

        /// <inheritdoc />
        public override ClientRequest GetClientRequest(HttpContext context)
        {
            /* How to get remove IP address:
                https://stackoverflow.com/questions/36352215/asp-net-core-how-to-get-remote-ip-address
              */

            var request = new ClientRequest();

            if (Options.ConfigureRequestAction != null)
            {
                Options.ConfigureRequestAction(context, request);
            }
            else
            {
                request.ClientId = context.Connection.RemoteIpAddress.ToString();
            }

            request.HttpVerb = request.HttpVerb ?? context.Request.Method.ToLower();
            request.Path = request.Path ?? context.Request.Path.ToString().ToLowerInvariant();

            return request;
        }
    }
}
