# Delobytes.Extensions.Configuration

[RU](README.md), [EN](README.en.md)

Расширения конфигурации .Net для поставщиков параметров/секретов. Пакет позволяет использовать следующие клиенты внешних сервисов в качестве источника конфигурации/секретов вашего .NetCore приложения (с помощью Microsoft.Extensions.Configuration):
- Яндекс.Облако Локбокс
- AWS AppConfig

## Установка

Для добавления пакета в ваше приложение вы можете использовать [NuGet](https://www.nuget.org/packages/Delobytes.Extensions.Configuration):

    dotnet add package Delobytes.Extensions.Configuration

## Использование

### Yandex Cloud Lockbox
Добавляет конфигурацию/секреты из сервиса Яндекс.Облако Lockbox.

1. Откройте консоль Яндекс.Облака и создайте сервисную учётную запись с ролью "lockbox.payloadViewer" и запишите её идентификатор.

2. Создайте новый авторизованный ключ для этой сервисной учётки. Запишите идентификатор ключа и приватный ключ.

3. Добавьте любой секрет в Локбокс. Для создания иерархии используйте какой-либо допустимый символ-разделитель.
  
![добавление секрета](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/add-lockbox-secret-ru.png)

4. После создания вам станет доступен идентификатор секрета. Занесите в настройки приложения (appsettings.json) идентификаторы:

```json
{
  "YC": {
    "ConfigurationSecretId": "e6q9a81c6m2bolpjaqjq",
    "ServiceAccountId": "ajm2bdb9qq3mk4umqq23",
    "ServiceAccountAuthorizedKeyId": "aje25rj0oacm5o10ib43"
  }
}
```

5. Добавьте источник конфигурации c помощью вызова метода расширения. Получите идентификаторы из файла конфигурации приложения, а приватный ключ из переменных среды и сконфигурируйте другие необходимые настройки:

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

6. В результате вы сможете получать объект стандартными методами работы с конфигурацией. Например, создав соответствующий объект и подключая к нему конфигурацию:

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

### AWS App Configuration
Добавляет конфигурацию/секреты из сервиса AWS AppConfig.

1. Получите ключ доступа и секретный ключ в AWS для соответствующего сервисного пользователя. Предоставьте эти данные приложению удобным для вас способом (например, через переменные окружения). Убедитесь, что у сервисного пользователя достаточно прав на чтение конфигураций AppConfig.

2. Укажите регион AWS из которого следует брать конфигурацию. Его можно добавить в настройки приложения (appsettings.json):

```json
{
  "AWS": {
    "Region": "us-east-1"
  }
}
```

3. Добавьте приложение, среду, и конфигурационный профиль c параметрами/секретами в консоли AWS AppConfig.

4. Добавьте источник конфигурации c помощью вызова метода расширения и предоставьте необходимые настройки:  

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

5. В результате вы сможете получать объект стандартными методами работы с конфигурацией. Например, создав соответствующий объект и подключая к нему конфигурацию:

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

## Лицензия
[МИТ](https://github.com/a-postx/Delobytes.Extensions.Configuration/blob/main/LICENSE)