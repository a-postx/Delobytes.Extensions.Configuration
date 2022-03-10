# Delobytes.Extensions.Configuration

[RU](README.md), [EN](README.en.md)

Расширения конфигурации .Net для поставщиков параметров/секретов. Пакет позволяет использовать следующие внешние сервисы в качестве источника конфигурации/секретов вашего .NetCore приложения (с помощью Microsoft.Extensions.Configuration):
- Яндекс.Облако Локбокс
- AWS AppConfig

## Установка

Для добавления пакета в ваше приложение вы можете использовать [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Использование

### Yandex Cloud Lockbox
Добавляет конфигурацию/секреты из сервиса Яндекс.Облака Lockbox.

1. Получите Oauth-токен доступа для Облака используя документацию: https://cloud.yandex.ru/docs/iam/concepts/authorization/oauth-token

2. Добавьте секрет в локбокс. Для создания иерархии используйте какой-либо допустимый символ-разделитель.
  
![добавление секрета](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/add-lockbox-secret-ru.png)

3. После создания вам станет доступен идентификатор секрета, его можно добавить в настройки приложения (appsettings.json):

```json
{
  "YC": {
    "ConfigurationSecretId": "a6q9d19c6m2a7lpjambd"
  }
}
```

4. Создайте объект, который будет представлять ваши настройки или секреты:

```csharp
public class AppSecrets
{
    public string SecretServiceToken { get; set; }
}
```

5. Добавьте источник конфигурации c помощью вызова метода расширения на IHostBuilder и предоставьте oauth-токен и другие необходимые настройки:  

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

6. Привяжите вашу конфигурацию к объекту:

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

7. Сразу после привязки вы сможете получать объект стандартными методами работы с конфигурацией, например:

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
[МИТ](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/LICENSE)