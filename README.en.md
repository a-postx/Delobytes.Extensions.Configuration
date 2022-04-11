# Delobytes.Extensions.Configuration

[RU](README.md), [EN](README.en.md)

.Net configuration extensions for configuration/secrets providers. Package allows to use the following third-party provider clients as a configuration source for your .NetCore application (via Microsoft.Extensions.Configuration):
- Yandex.Cloud Lockbox
- AWS AppConfig

## Installation

The fastest way to add package to your app is via [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Usage

### Yandex Cloud Lockbox
Add configuration/secrets from Yandex Cloud Lockbox service.

1. Go to Yandex.Cloud console and create new service account with role "lockbox.payloadViewer" to get service account ID.

2. Create new authorized key for this service account to get key identifier and private key.

3. Go to Lockbox and add a secret. Use some allowed delimiter to create your hierarchy:
  
![adding a secret to Lockbox](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/add-lockbox-secret-en.png)

4. Once you created a secret you will get secret identifier. Add identifiers to the application settings (appsettings.json):

```json
{
  "YC": {
    "ConfigurationSecretId": "e6q9a81c6m2bolpjaqjq",
    "ServiceAccountId": "ajm2bdb9qq3mk4umqq23",
    "ServiceAccountAuthorizedKeyId": "aje25rj0oacm5o10ib43"
  }
}
```

5. Add confguration source using extension method. Get identifiers from the application settings file and private key using some environment variable. Configure all other settings as needed:  

```csharp
IHostBuilder hostBuilder = new HostBuilder().UseContentRoot(Directory.GetCurrentDirectory());

hostBuilder.ConfigureAppConfiguration(configBuilder =>
{
    IConfigurationRoot tempConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
			
    configBuilder.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = Environment.GetEnvironmentVariable("YC_PRIVATE_KEY");
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
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

6. Now you can get your secrets using standard methods. For example, by creating an object representing your secrets and binding configuration to this object:

```csharp
public class AppSecrets
{
    public string SecretServiceToken { get; set; }
}
```

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

1. Create AccessKey and SecretAccessKey in AWS for your service account and provide this data to your application (for example, using environment variables). Make sure that service account has rights to read AppConfig configurations.

2. Add region where the configuration should be picked up from. You can add it using application settings (appsettings.json):

```json
{
  "AWS": {
    "Region": "us-east-1"
  }
}
```

3. Add application, environment and configuration profile with parameters in AppConfig.

4. Add confguration source using extension method. Apply your RegionEndpoint and other settings:   

```csharp
IHostBuilder hostBuilder = new HostBuilder().UseContentRoot(Directory.GetCurrentDirectory());

hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
{
    IHostEnvironment hostEnvironment = hostingContext.HostingEnvironment;
	IConfigurationRoot tempConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

    configBuilder.AddAwsAppConfigConfiguration(config =>
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(tempConfig.GetValue<string>("AWS:Region"));
            config.EnvironmentName = hostEnvironment.EnvironmentName;
            config.ApplicationName = hostEnvironment.ApplicationName;
            config.ConfigurationName = $"{hostEnvironment.EnvironmentName}-{hostEnvironment.ApplicationName}-profile";
            config.ClientId = $"{hostEnvironment.ApplicationName}-{Node.Id}";
            config.Optional = false;
            config.ReloadPeriod = TimeSpan.FromDays(1);
            config.LoadTimeout = TimeSpan.FromSeconds(20);
            config.OnLoadException += exceptionContext =>
            {
                //log
            };
        });
})
```

5. Now you can get your secrets using standard methods. For example, by creating an object representing your secrets and binding configuration to this object:

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

## License
[MIT](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/LICENSE)