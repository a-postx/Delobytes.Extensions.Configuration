# Delobytes.Extensions.Configuration
Расширения конфигурации .Net для популярных внешних поставщиков параметров/секретов.

[RU](README.md), [EN](README.en.md)

## Установка

Для добавления пакета в ваше приложение вы можете использовать [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Использование

### Yandex Cloud Lockbox
Добавляет конфигурацию/секреты из сервиса Yandex.Cloud Lockbox.

Добавьте источник конфигурации c помощью вызова метода расширения на IHostBuilder:  

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

### AWS App Configuration
Добавляет конфигурацию/секреты из сервиса AWS AppConfig.

Добавьте источник конфигурации:

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



## Лицензия
[МИТ](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/master/LICENSE)