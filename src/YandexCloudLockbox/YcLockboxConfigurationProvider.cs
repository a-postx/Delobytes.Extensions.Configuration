using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Yandex.Cloud;
using Yandex.Cloud.Credentials;
using Yandex.Cloud.Lockbox.V1;
using static Yandex.Cloud.Lockbox.V1.Payload.Types;

namespace Delobytes.Extensions.Configuration.YandexCloudLockbox;

public class YcLockboxConfigurationProvider : ConfigurationProvider
{
    public YcLockboxConfigurationProvider(YcLockboxConfigurationSource source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrEmpty(source.OauthToken))
        {
            throw new ArgumentException("Oauth token cannot be found.", nameof(source.OauthToken));
        }

        _sdk = new Sdk(new OAuthCredentialsProvider(source.OauthToken));

        _optional = source.Optional;
        _secretId = source.SecretId;
        _path = source.Path;
        _pathSeparator = source.PathSeparator;
        _reloadPeriod = source.ReloadPeriod;
        _loadTimeout = source.LoadTimeout;
        _onLoadException = source.OnLoadException;

        ChangeToken.OnChange(() =>
        {
            CancellationTokenSource cts = new CancellationTokenSource(_reloadPeriod);
            CancellationChangeToken cancellationChangeToken = new CancellationChangeToken(cts.Token);
            return cancellationChangeToken;
#pragma warning disable VSTHRD101 //Пока нет асинхронного метода https://github.com/dotnet/runtime/issues/36018
        }, async () => await LoadAsync(true));
#pragma warning restore VSTHRD101
    }

    private readonly bool _optional;
    private readonly string _secretId;
    private readonly string _path;
    private readonly char _pathSeparator;
    private readonly TimeSpan _reloadPeriod;
    private readonly TimeSpan _loadTimeout;
    private readonly Action<YcLockboxExceptionContext> _onLoadException;

    private readonly Sdk _sdk;

    private const char ConfigurationKeyDelimiter = ':';


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
                YcLockboxExceptionContext exceptionContext = new YcLockboxExceptionContext
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

    private async Task<RepeatedField<Entry>> GetSecretAsync(string id, CancellationToken cancellationToken)
    {
        Payload payload = await _sdk.Services.Lockbox.PayloadService
            .GetAsync(new GetPayloadRequest { SecretId = id }, null, null, cancellationToken);

        if (payload.Entries.Count > 0)
        {
            return payload.Entries;
        }
        else
        {
            return new RepeatedField<Entry>();
        }
    }

    private async Task<Dictionary<string, string>> GetAllKeyValuePairsAsync(CancellationToken cancellationToken)
    {
        //ListCloudsResponse response = sdk.Services.Resourcemanager.CloudService.List(new ListCloudsRequest());
        //Cloud? prodCloud = response.Clouds.FirstOrDefault();

        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        RepeatedField<Entry> entries = await GetSecretAsync(_secretId, cancellationToken);

        if (entries.Count == 0)
        {
            return null;
        }

        foreach (Entry entry in entries)
        {
            Entry.ValueOneofCase value = entry.ValueCase;

            if (value == Entry.ValueOneofCase.TextValue)
            {
                string keyPath = entry.Key;

                if (!string.IsNullOrEmpty(_path) && keyPath.StartsWith(_path, StringComparison.OrdinalIgnoreCase))
                {
                    keyPath = keyPath.Substring(_path.Length).TrimStart(_pathSeparator);
                }
                
                keyPath = keyPath.Replace(_pathSeparator, ConfigurationKeyDelimiter);

                result.Add(keyPath, entry.TextValue);
            }
            else if (value == Entry.ValueOneofCase.BinaryValue)
            {
                throw new NotSupportedException("Binary secret key type is not supported");
            }
            else
            {
                throw new NotSupportedException("Unknown secret key type");
            }
        }

        return result;
    }
}
