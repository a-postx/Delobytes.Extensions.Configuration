# Delobytes.Extensions.Configuration
.Net configuration extensions for configuration/secrets providers. Package allows to use the following third-party providers as a configuration source for your dotnet application (via Microsoft.Extensions.Configuration):
- Yandex.Cloud Lockbox
- AWS AppConfig

[RU](README.md), [EN](README.en.md)

## Installation

The fastest way to add package to your app is via [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Usage

### Yandex Cloud Lockbox
Add configuration/secrets from Yandex Cloud Lockbox service.

1. Obtain Yandex Cloud oauth access token by following the guide: https://cloud.yandex.com/en/docs/iam/concepts/authorization/oauth-token

2. Add secret to Lockbox. Use some allowed delimiter to create your hierarchy:
![adding a secret to Lockbox](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/add-lockbox-secret-en.png)

3. Once you created a secret you will get secret identifier. You can add it to the application settings (appsettings.json):

```json
{
  "YC": {
    "ConfigurationSecretId": "a6q9d19c6m2a7lpjambd"
  }
}
```

4. Create an object that will represent your settings or secrets:

```csharp
public class AppSecrets
{
    public string SecretServiceToken { get; set; }
}
```

5. Add confguration source using extension method of IHostBuilder. Apply your oauth token and other settings:  

```csharp
IHostBuilder hostBuilder = new HostBuilder().UseContentRoot(Directory.GetCurrentDirectory());

hostBuilder.ConfigureAppConfiguration(configBuilder =>
{
    IConfigurationRoot tempConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
			
    configBuilder.AddYandexCloudLockboxConfiguration(config =>
        {
            config.OauthToken = Environment.GetEnvironmentVariable("YC_OAUTH_TOKEN");
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.Path = "MyPath";
            config.PathSeparator = '-';
            config.Optional = false;
            config.ReloadPeriod = TimeSpan.FromDays(7);
            config.LoadTimeout = TimeSpan.FromSeconds(20);
            config.OnLoadException += exceptionContext =>
            {
                //log
            };
        });
})
```

6. Bind configuration to your object:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private readonly IConfiguration _config;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .Configure<AppSecrets>(_config.GetSection(nameof(AppSecrets)), o => o.BindNonPublicProperties = false);
    }
}
```

7. Now you can get your secrets using standard methods:

```csharp
[Route("/")]
[ApiController]
public class HomeController : ControllerBase
{
    public HomeController(IConfiguration config)
    {
        _config = config;
    }

    private readonly IConfiguration _config;

    [HttpGet("")]
    public IActionResult Get()
    {
        AppSecrets secrets = _config.GetSection(nameof(AppSecrets)).Get<AppSecrets>();

        return Ok();
    }
}

```

### AWS App Config
Add configuration/secrets from AWS AppConfig service.

Add configuration source:

```csharp
builder.ConfigureAppConfiguration(configBuilder =>
{
    configBuilder.AddAwsAppConfigConfiguration(config =>
        {
			config.RegionEndpoint = RegionEndpoint.GetBySystemName(tempConfig.GetValue<string>("AWS:Region"));
            config.EnvironmentName = hostingEnvironment.EnvironmentName;
            config.ApplicationName = hostingEnvironment.ApplicationName;
            config.ConfigurationName = $"{hostingEnvironment.EnvironmentName}-{hostingEnvironment.ApplicationName}-profile";
            config.ClientId = $"{hostingEnvironment.ApplicationName}-{Node.Id}";
            config.Optional = false;
            config.ReloadPeriod = TimeSpan.FromDays(7);
            config.LoadTimeout = TimeSpan.FromSeconds(20);
            config.OnLoadException += exceptionContext =>
            {
                //log
            };
        });
})
```


## License
[MIT](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/LICENSE)