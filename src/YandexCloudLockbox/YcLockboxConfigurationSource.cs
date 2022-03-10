using System;
using Microsoft.Extensions.Configuration;

namespace Delobytes.Extensions.Configuration.YandexCloudLockbox;

/// <summary>
/// Настройки конфигурации с помощью Yandex.Cloud Lockbox
/// </summary>
public class YcLockboxConfigurationSource : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new YcLockboxConfigurationProvider(this);
    }


    /// <summary>
    /// Токен аутентификации.
    /// </summary>
    public string OauthToken { get; set; }
    /// <summary>
    /// Идентификатор секрета из которого необходимо взять элементы конфигурации.
    /// </summary>
    public string SecretId { get; set; }
    /// <summary>
    /// Префикс в ключах с которого будет начинаться формирование конфигурации.
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Разделитель путей в иерархии параметров. Разделитель ":" является запрещённым
    /// символом в ключах локбокса, поэтому необходимо заменить его другим.
    /// </summary>
    public char PathSeparator { get; set; }
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
    /// Будет вызван, если произошло необработанное исключение при вызове загрузки конфигурации.
    /// </summary>
    public Action<YcLockboxExceptionContext> OnLoadException { get; set; }
}
