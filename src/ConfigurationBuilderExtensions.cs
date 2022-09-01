using System;
using Delobytes.Extensions.Configuration.AwsAppConfig;
using Delobytes.Extensions.Configuration.YandexCloudLockbox;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Расширения строителя конфигурации.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Добавляет провайдер секретов на базе AWS AppConfiguration.
    /// </summary>
    /// <param name="builder">Строитель конфигурации.</param>
    /// <param name="configureSource">Конфигурационный вызов.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Аргумент недоступен.</exception>
    public static IConfigurationBuilder AddAwsAppConfigConfiguration(this IConfigurationBuilder builder, Action<AwsAppConfigConfigurationSource> configureSource)
    {
        ArgumentNullException.ThrowIfNull(configureSource, nameof(configureSource));

        AwsAppConfigConfigurationSource source = new AwsAppConfigConfigurationSource();
        configureSource(source);

        if (string.IsNullOrEmpty(source.EnvironmentName))
        {
            throw new ArgumentNullException(nameof(source.EnvironmentName));
        }

        if (string.IsNullOrEmpty(source.ApplicationName))
        {
            throw new ArgumentNullException(nameof(source.ApplicationName));
        }

        if (string.IsNullOrEmpty(source.ConfigurationName))
        {
            throw new ArgumentNullException(nameof(source.ConfigurationName));
        }

        if (string.IsNullOrEmpty(source.ClientId))
        {
            throw new ArgumentNullException(nameof(source.ClientId));
        }

        builder.Add(source);
        return builder;
    }

    /// <summary>
    /// Добавляет провайдер секретов на базе Lockbox.
    /// </summary>
    /// <param name="builder">Строитель конфигурации.</param>
    /// <param name="configureSource">Конфигурационный вызов.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Аргумент недоступен.</exception>
    public static IConfigurationBuilder AddYandexCloudLockboxConfiguration(this IConfigurationBuilder builder, Action<YcLockboxConfigurationSource> configureSource)
    {
        ArgumentNullException.ThrowIfNull(configureSource, nameof(configureSource));

        YcLockboxConfigurationSource source = new YcLockboxConfigurationSource();
        configureSource(source);

        if (string.IsNullOrEmpty(source.ServiceAccountId))
        {
            throw new ArgumentNullException(nameof(source.ServiceAccountId));
        }
        
        if (string.IsNullOrEmpty(source.ServiceAccountAuthorizedKeyId))
        {
            throw new ArgumentNullException(nameof(source.ServiceAccountAuthorizedKeyId));
        }

        if (string.IsNullOrEmpty(source.PrivateKey))
        {
            throw new ArgumentNullException(nameof(source.PrivateKey));
        }

        if (string.IsNullOrEmpty(source.SecretId))
        {
            throw new ArgumentNullException(nameof(source.SecretId));
        }

        if (source.PathSeparator == new char())
        {
            throw new ArgumentNullException(nameof(source.PathSeparator));
        }

        builder.Add(source);
        return builder;
    }
}
