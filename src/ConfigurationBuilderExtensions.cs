using System;
using Delobytes.Extensions.Configuration.AwsAppConfig;
using Delobytes.Extensions.Configuration.YandexCloudLockbox;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddAwsAppConfigConfiguration(this IConfigurationBuilder builder, Action<AwsAppConfigConfigurationSource> configureSource)
    {
        if (configureSource == null)
        {
            throw new ArgumentNullException(nameof(configureSource));
        }

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

    public static IConfigurationBuilder AddYandexCloudLockboxConfiguration(this IConfigurationBuilder builder, Action<YcLockboxConfigurationSource> configureSource)
    {
        if (configureSource == null)
        {
            throw new ArgumentNullException(nameof(configureSource));
        }
        
        YcLockboxConfigurationSource source = new YcLockboxConfigurationSource();
        configureSource(source);

        if (string.IsNullOrEmpty(source.OauthToken))
        {
            throw new ArgumentNullException(nameof(source.OauthToken));
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
