# AspNetThrottle
ASP.NET Core request throttle

## Install from NuGet
In the Package Manager Console:

`PM> Install-Package AspNetThrottle.NetCore`

## Use middleware in Startup.cs

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddThrottle()
            .AddMemoryCacheCounterStore();

        //...
    }
```

```csharp
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        //...

        /*
        * You can use either of IP or ID throttle as of below. Or both.
        */

        // IP address throttle.
        var ipOptions = new ThrottleOptions();
        Configuration.GetSection("IpThrottle").Bind(ipOptions);
        app.UseIpThrottle(ipOptions);

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

        //...
    }
```

## Configuration by appsettings.json

### IP throttle

```json
  "IpThrottle": {
    "ThrottleName": "myIpThrottle",
    "HttpStatusCode": 403,
    "QuotaExceededMessage": null,
    "ClientWhitelist": [ "10.10.1.100-10.10.1.255", "192.168.0.0/24" ],
    "EndpointWhitelist": [ "delete:/api/values", "*:/api/another" ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "15s",
        "Limit": 2
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 3
      },
      {
        "Endpoint": "post:/api/values",
        "Period": "2m",
        "Limit": 3
      }
    ],
    "ClientPolicies": [
      {
        "ClientId": "::1",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "8s",
            "Limit": 2
          }
        ]
      },
      {
        "ClientId": "192.168.0.1",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "9s",
            "Limit": 3
          }
        ]
      }
    ]
  }
```

### ID throttle
```json
  "IdThrottle": {
    "ThrottleName": "myIdThrottle",
    "HttpStatusCode": 401,
    "QuotaExceededMessage": "Quota on your ID exceeded!",
    "ClientWhitelist": [ "David", "Donny" ],
    "EndpointWhitelist": [ "delete:/api/values", "*:/api/another" ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "15s",
        "Limit": 2
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 3
      },
      {
        "Endpoint": "post:/api/values",
        "Period": "2m",
        "Limit": 3
      }
    ],
    "ClientPolicies": [
      {
        "ClientId": "Alice",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "11s",
            "Cooldown": "20s",
            "Limit": 2
          }
        ]
      },
      {
        "ClientId": "Bob",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "12s",
            "Limit": 3
          }
        ]
      }
    ]
  }
```