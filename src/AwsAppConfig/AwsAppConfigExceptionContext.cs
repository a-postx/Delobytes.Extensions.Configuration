using System;

namespace Delobytes.Extensions.Configuration.AwsAppConfig;

/// <summary>
/// Содержит информацию об исключении во время загрузки флагов.
/// </summary>
public class AwsAppConfigExceptionContext
{
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
