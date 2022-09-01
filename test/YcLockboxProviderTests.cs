using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Delobytes.Extensions.Configuration.Tests;

public class YcLockboxProviderTests
{
    private static readonly string RequestPath = "/json";

    #region Infrastructure
    private string GetPrivateKey()
    {
        string? privateKey = Environment.GetEnvironmentVariable("YC_PRIVATE_KEY");

        if (string.IsNullOrEmpty(privateKey))
        {
            throw new InvalidOperationException("Private key is not available");
        }

        return privateKey;
    }

    private IConfigurationRoot GetValidConfiguration()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        return config;
    }

    private AppSecrets? GetAppSecrets()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = GetPrivateKey();
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.PathSeparator = '-';
            config.Optional = false;
            config.ReloadPeriod = TimeSpan.FromDays(7);
            config.LoadTimeout = TimeSpan.FromSeconds(20);
            config.OnLoadException += exceptionContext =>
            {
                //log
            };
        });

        builder.Services.AddSingleton<IValidateOptions<AppSecrets>, AppSecretsValidator>();
        builder.Services
            .Configure<AppSecrets>(builder.Configuration.GetSection(nameof(AppSecrets)), o => o.BindNonPublicProperties = false);
        AppSecrets? appSecrets = builder.Services.BuildServiceProvider().GetService<IOptions<AppSecrets>>()?.Value;

        return appSecrets;
    }
    #endregion

    [Fact]
    public void YcLockboxProvider_Throws_WithEmptyPrivateKey()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action configureOptions = () => builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = default!;
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.PathSeparator = '-';
            config.Optional = false;
        });

        Exception ex = Record.Exception(configureOptions);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_Throws_WithEmptyServiceAccountId()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action configureOptions = () => builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = GetPrivateKey();
            config.ServiceAccountId = default!;
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.PathSeparator = '-';
            config.Optional = false;
        });

        Exception ex = Record.Exception(configureOptions);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_Throws_WithEmptyServiceAccountAuthorizedKeyId()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action configureOptions = () => builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = GetPrivateKey();
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = default!;
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.PathSeparator = '-';
            config.Optional = false;
        });

        Exception ex = Record.Exception(configureOptions);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_Throws_WithEmptySecretId()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action configureOptions = () => builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = GetPrivateKey();
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
            config.SecretId = default!;
            config.PathSeparator = '-';
            config.Optional = false;
        });

        Exception ex = Record.Exception(configureOptions);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_Throws_WithEmptyPathSeparator()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action configureOptions = () => builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
        {
            config.PrivateKey = GetPrivateKey();
            config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
            config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
            config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
            config.PathSeparator = default;
            config.Optional = false;
        });

        Exception ex = Record.Exception(configureOptions);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_ReceivesConfiguration()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action getConfiguration = () => {
            builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
            {
                config.PrivateKey = GetPrivateKey();
                config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
                config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
                config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
                config.PathSeparator = '-';
                config.Optional = false;
            });
        };

        Exception ex = Record.Exception(getConfiguration);

        ex.Should().BeNull();
    }

    [Fact]
    public void YcLockboxProvider_Throws_WhenCannotRecieveConfiguration_AndNotOptional()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action getConfiguration = () => {
            builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
            {
                config.PrivateKey = "123";
                config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
                config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
                config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
                config.PathSeparator = '-';
                config.Optional = false;
            });
        };

        Exception ex = Record.Exception(getConfiguration);

        ex.Should().NotBeNull();
    }

    [Fact]
    public void YcLockboxProvider_DoNotThrows_WhenCannotRecieveConfiguration_AndOptional()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action getConfiguration = () => {
            builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
            {
                config.PrivateKey = "123";
                config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
                config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
                config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
                config.PathSeparator = '-';
                config.Optional = true;
            });
        };

        Exception ex = Record.Exception(getConfiguration);

        ex.Should().BeNull();
    }

    [Fact]
    public void YcLockboxProvider_RecievesConfiguration_AndProvidesToApplication()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        IConfigurationRoot tempConfig = GetValidConfiguration();

        Action getConfiguration = () =>
        {
            builder.Configuration.AddYandexCloudLockboxConfiguration(config =>
            {
                config.PrivateKey = GetPrivateKey();
                config.ServiceAccountId = tempConfig.GetValue<string>("YC:ServiceAccountId");
                config.ServiceAccountAuthorizedKeyId = tempConfig.GetValue<string>("YC:ServiceAccountAuthorizedKeyId");
                config.SecretId = tempConfig.GetValue<string>("YC:ConfigurationSecretId");
                config.PathSeparator = '-';
                config.Optional = false;
            });
        };

        Exception getConfigEx = Record.Exception(getConfiguration);

        AppSecrets? appSecrets = null;

        Action getAppSecrets = () => {
            builder.Services.AddSingleton<IValidateOptions<AppSecrets>, AppSecretsValidator>();
            builder.Services
                .Configure<AppSecrets>(builder.Configuration.GetSection(nameof(AppSecrets)), o => o.BindNonPublicProperties = false);
            appSecrets = builder.Services.BuildServiceProvider().GetService<IOptions<AppSecrets>>()?.Value;
        };

        Exception materializeAppSecretsEx = Record.Exception(getAppSecrets);

        getConfigEx.Should().BeNull();
        materializeAppSecretsEx.Should().BeNull();
        appSecrets.Should().NotBeNull();
        appSecrets!.TestString.Should().NotBeNullOrEmpty();
        appSecrets!.TestInt.Should().NotBeNull();
        appSecrets!.TestBool.Should().NotBeNull();
        appSecrets!.TestGuid.Should().NotBeNull();
        appSecrets!.TestObject.Should().NotBeNull();
        appSecrets!.TestObject!.TestString.Should().NotBeNullOrEmpty();
    }
}
