using System;

namespace Delobytes.Extensions.Configuration.YandexCloudLockbox;

/// <summary>
/// Содержит информацию об исключении во время загрузки флагов.
/// </summary>
public class YcLockboxExceptionContext
{
    /// <summary>
    /// <see cref="YcLockboxConfigurationProvider" /> который вызвал исключение.
    /// </summary>
    public YcLockboxConfigurationProvider Provider { get; set; }

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
