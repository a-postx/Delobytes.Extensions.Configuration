using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Delobytes.Extensions.Configuration.AwsAppConfig;

public class AwsAppConfigConfigurationProvider : ConfigurationProvider
{
    public AwsAppConfigConfigurationProvider(AwsAppConfigConfigurationSource source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _appConfigClient = new AmazonAppConfigClient(new AmazonAppConfigConfig
        {
            RegionEndpoint = source.RegionEndpoint ?? RegionEndpoint.USEast1
        });

        _environmentName = source.EnvironmentName.ToLowerInvariant();
        _applicationName = source.ApplicationName.ToLowerInvariant();
        _configurationName = source.ConfigurationName.ToLowerInvariant();
        _clientId = source.ClientId.ToLowerInvariant();
        _optional = source.Optional;
        _reloadPeriod = source.ReloadPeriod;
        _loadTimeout = source.LoadTimeout;
        _onLoadException = source.OnLoadException;

        ChangeToken.OnChange(() =>
        {
            CancellationTokenSource cts = new CancellationTokenSource(_reloadPeriod);
            CancellationChangeToken cancellationChangeToken = new CancellationChangeToken(cts.Token);
            return cancellationChangeToken;
#pragma warning disable VSTHRD101 //Пока нет асинхронного метода https://github.com/dotnet/runtime/issues/36018
        }, async () => { await LoadAsync(true); });
#pragma warning restore VSTHRD101
    }

    private static string _awsConfigurationVersion = "0";
    private static AmazonAppConfigClient _appConfigClient;

    private readonly string _environmentName;
    private readonly string _applicationName;
    private readonly string _configurationName;
    private readonly string _clientId;
    private readonly bool _optional;
    private readonly TimeSpan _reloadPeriod;
    private readonly TimeSpan _loadTimeout;
    private readonly Action<AwsAppConfigExceptionContext> _onLoadException;

    //Пока нет асинхронного метода https://github.com/dotnet/runtime/issues/36018
    public override void Load()
    {
#pragma warning disable VSTHRD002 //Пока нет асинхронного метода https://github.com/dotnet/runtime/issues/36018
        LoadAsync(false).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
    }

    private async Task LoadAsync(bool reload)
    {
        try
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(_loadTimeout))
            {
                Dictionary<string, string> kvPairs = await GetAllKeyValuePairsAsync(cts.Token);

                if (kvPairs != null)
                {
                    if (!Data.ContentEquals(kvPairs))
                    {
                        Data = kvPairs;
                        OnReload();
                    }
                };
            }
        }
        catch (Exception ex)
        {
            if (_optional)
            {
                return;
            }

            bool ignoreException = reload;

            if (_onLoadException != null)
            {
                AwsAppConfigExceptionContext exceptionContext = new AwsAppConfigExceptionContext
                {
                    Provider = this,
                    Exception = ex,
                    Reload = reload
                };

                _onLoadException(exceptionContext);
                ignoreException = exceptionContext.Ignore;
            }

            if (!ignoreException)
            {
                throw;
            }
        }
    }

    private async Task<Dictionary<string, string>> GetAllKeyValuePairsAsync(CancellationToken cancellationToken)
    {
        GetConfigurationRequest request = new GetConfigurationRequest
        {
            Application = _applicationName,
            ClientConfigurationVersion = _awsConfigurationVersion,
            ClientId = _clientId,
            Configuration = _configurationName,
            Environment = _environmentName
        };

        GetConfigurationResponse configResponse = await _appConfigClient.GetConfigurationAsync(request, cancellationToken);
            
        if (_awsConfigurationVersion == configResponse.ConfigurationVersion)
        {
            return null;
        }

        Dictionary<string, string> result = await DeserializeDataAsync(configResponse.Content);

        _awsConfigurationVersion = configResponse.ConfigurationVersion;

        return result;
    }

    private async Task<Dictionary<string, string>> DeserializeDataAsync(MemoryStream stream)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
            
        string json = null;

        using (StreamReader reader = new StreamReader(stream))
        {
            json = await reader.ReadToEndAsync();
        }
            
        if (!string.IsNullOrEmpty(json))
        {
            result = GetJsonAsConfiguration(json);
        }

        return result;
    }

    private Dictionary<string, string> GetJsonAsConfiguration(string json)
    {
        IEnumerable<(string Path, string P)> GetLeaves(string path, JsonProperty p)
        {
            return p.Value.ValueKind != JsonValueKind.Object
                                ? new[] { (Path: path == null ? p.Name : path + ":" + p.Name, p.Value.ToString()) }
                                : p.Value.EnumerateObject().SelectMany(child => GetLeaves(path == null ? p.Name : path + ":" + p.Name, child));
        }

        using (JsonDocument document = JsonDocument.Parse(json))
        {
            return document.RootElement.EnumerateObject()
                .SelectMany(p => GetLeaves(null, p))
                .ToDictionary(k => k.Path, v => v.P);
        }
    }
}
