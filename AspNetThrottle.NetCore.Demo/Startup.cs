using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetThrottle.NetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThrottle()
                .AddMemoryCacheCounterStore();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // For demo purpose - set client ID directly from query string's "username" parameter.
            app.Use((httpContext, next) =>
            {
                var userName = httpContext.Request.Query["username"];

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "myCustomScheme"));
                }
                return next();
            });

            /*
             * You can use either of IP or ID throttle as of below. Or both.
             */

            // IP address throttle.
            var ipOptions = new ThrottleOptions();
            Configuration.GetSection("IpThrottle").Bind(ipOptions);
            //app.UseIpThrottle(ipOptions);

            // ID throttle.
            var idOptions = new ThrottleOptions();
            Configuration.GetSection("IdThrottle").Bind(idOptions);
            idOptions.ConfigureRequest((context, request) =>
            {
                // Demo for custom ID setting.
                // Actually this is the default implementation.
                request.ClientId = context.User?.Identity?.Name;
            });
            app.UseIdThrottle(idOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
