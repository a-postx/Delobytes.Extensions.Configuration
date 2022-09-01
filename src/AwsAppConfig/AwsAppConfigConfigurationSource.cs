using System;
using Amazon;
using Microsoft.Extensions.Configuration;

namespace Delobytes.Extensions.Configuration.AwsAppConfig;

/// <summary>
/// Настройки конфигурации с помощью AWS AppConfig
/// </summary>
public class AwsAppConfigConfigurationSource : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AwsAppConfigConfigurationProvider(this);
    }


    /// <summary>
    /// Имя приложения.
    /// </summary>
    public string ApplicationName { get; set; } = default!;
    /// <summary>
    /// Имя конфигурации.
    /// </summary>
    public string ConfigurationName { get; set; } = default!;
    /// <summary>
    /// Имя среды.
    /// </summary>
    public string EnvironmentName { get; set; } = default!;
    /// <summary>
    /// Идентификатор приложения, которое запрашивает конфигурацию.
    /// </summary>
    public string ClientId { get; set; } = default!;
    /// <summary>
    /// Признак того, что конфигурация является опциональной. Если стоит значение false,
    /// то при невозможности загрузки конфигурации будет вызвано исключение.
    /// </summary>
    public bool Optional { get; set; }
    /// <summary>
    /// Таймаут запроса загрузки конфигурации.
    /// </summary>
    public TimeSpan LoadTimeout { get; set; } = TimeSpan.FromSeconds(60);
    /// <summary>
    /// Период обновления конфигурации. По-умолчанию: 7 дней.
    /// </summary>
    public TimeSpan ReloadPeriod { get; set; } = TimeSpan.FromDays(7);
    /// <summary>
    /// Региональная точка доступа.
    /// </summary>
    public RegionEndpoint? RegionEndpoint { get; set; }
    /// <summary>
    /// Вызов, который будет запущен если произошло необработанное исключение при загрузке конфигурации.
    /// </summary>
    public Action<AwsAppConfigExceptionContext>? OnLoadException { get; set; }
}
