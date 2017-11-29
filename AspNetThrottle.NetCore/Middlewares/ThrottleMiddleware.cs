using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetThrottle.NetCore
{
    /// <summary>
    /// Represents an ASP.NET Core middleware to limit API access rate.
    /// </summary>
    public abstract class ThrottleMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleMiddleware"/> class.
        /// Makes this protected to prevent public call on an abstract class.
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="options">The throttle options.</param>
        /// <param name="counterStore">The store for throttle counters.</param>
        /// <param name="logger">The logger.</param>
        protected ThrottleMiddleware(RequestDelegate next, ThrottleOptions options, ICounterStore counterStore, ILogger logger)
        {
            Next = next;
            Options = options;
            CounterStore = counterStore;
            Logger = logger;
        }

        protected RequestDelegate Next { get; }

        protected ThrottleOptions Options { get; }

        protected ICounterStore CounterStore { get; }

        protected ILogger Logger { get; set; }

        protected ThrottleProcessor Processor { get; set; }

        public virtual async Task Invoke(HttpContext httpContext)
        {
            // Checks if throttle is enabled.
            if (Options == null)
            {
                await Next.Invoke(httpContext);
                return;
            }

            var request = GetClientRequest(httpContext);

            // Checks white list.
            if (Processor.IsWhitelisted(request))
            {
                await Next.Invoke(httpContext);
                return;
            }

            var rules = Processor.GetMatchingRules(request);

            foreach (var rule in rules)
            {
                if (rule.Limit <= 0)
                {
                    continue;
                }

                // Increments counter and sets indicator.
                var counter = Processor.ProcessRequest(request, rule);

                // Checks if the limit is exceeded.
                if (!counter.LimitExceeded)
                {
                    continue;
                }

                // Logs blocked request.
                LogBlockedRequest(httpContext, request, counter, rule);

                // Breaks execution and return error message.
                await ReturnQuotaExceededResponse(httpContext, rule);

                return;
            }

            await Next.Invoke(httpContext);
        }

        /// <summary>
        /// Gets the <see cref="ClientRequest"/> object based on specified context.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> object.</param>
        /// <returns>The <see cref="ClientRequest"/> object</returns>
        public abstract ClientRequest GetClientRequest(HttpContext context);

        public virtual void LogBlockedRequest(HttpContext httpContext, ClientRequest request, RequestCounter counter, ThrottleRule rule)
        {
            Logger.LogInformation($"Request {request.Path} from ClientId {request.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.TotalRequests}" +
                                   $". Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
        }

        public virtual Task ReturnQuotaExceededResponse(HttpContext httpContext, ThrottleRule rule)
        {
            var message = string.IsNullOrEmpty(Options.QuotaExceededMessage) ? $"API calls quota exceeded! maximum admitted {rule.Limit} per {rule.Period}." : Options.QuotaExceededMessage;

            httpContext.Response.StatusCode = Options.HttpStatusCode;

            return httpContext.Response.WriteAsync("{\"message\":\"" + message + "\"}");
        }
    }
}
