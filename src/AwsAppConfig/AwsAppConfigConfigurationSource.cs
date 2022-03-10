using System;
using Amazon;
using Microsoft.Extensions.Configuration;

namespace Delobytes.Extensions.Configuration.AwsAppConfig;

public class AwsAppConfigConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AwsAppConfigConfigurationProvider(this);
    }

    public string EnvironmentName { get; set; }
    public string ApplicationName { get; set; }
    public string ConfigurationName { get; set; }
    public string ClientId { get; set; }
    public bool Optional { get; set; }
    public TimeSpan ReloadPeriod { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan LoadTimeout { get; set; } = TimeSpan.FromSeconds(60);
    public RegionEndpoint RegionEndpoint { get; set; }
    /// <summary>
    /// Будет вызван, если произошло необработанное исключение при вызове загрузки конфигурации.
    /// </summary>
    public Action<AwsAppConfigExceptionContext> OnLoadException { get; set; }
}
