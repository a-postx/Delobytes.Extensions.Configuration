using System;

namespace Delobytes.Extensions.Configuration.AwsAppConfig;

/// <summary>
/// Содержит информацию об исключении во время загрузки флагов.
/// </summary>
public class AwsAppConfigExceptionContext
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="provider">Провайдер конфигурации.</param>
    /// <param name="exception">Исключение.</param>
    /// <param name="reload">Флаг обозначающий выброс исключения во время обновления конфигурации.</param>
    /// <exception cref="ArgumentNullException">Аргумент не найден.</exception>
    public AwsAppConfigExceptionContext(AwsAppConfigConfigurationProvider provider, Exception exception, bool reload)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));
        ArgumentNullException.ThrowIfNull(reload, nameof(reload));

        Provider = provider;
        Exception = exception;
        Reload = reload;
    }

    /// <summary>
    /// <see cref="AwsAppConfigConfigurationProvider" /> который вызвал исключение.
    /// </summary>
    public AwsAppConfigConfigurationProvider Provider { get; set; }

    /// <summary>
    /// Исключение, произошедшее при загрузке.
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// Если true, то исключение не будет перевыброшено.
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Если true, то исключение было выброшено во время обновления конфигурации.
    /// </summary>
    public bool Reload { get; set; }
}
