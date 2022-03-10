# Delobytes.Extensions.Configuration
.Net configuration extensions for popular t configuration/secrets providers.

[RU](README.md), [EN](README.en.md)

## Installation

The fastest way to add package to your app is via [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Usage

### Yandex Cloud Lockbox
Add configuration/secrets from Yandex Cloud Lockbox service.

Call the extension method on the IHostBuilder with your options:

```csharp
builder.ConfigureAppConfiguration(configBuilder =>
{
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
[MIT](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/master/LICENSE)