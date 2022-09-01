using System;
using Grpc.Core;
using Yandex.Cloud.Credentials;
using Yandex.Cloud.Iam.V1;

namespace Delobytes.Extensions.Configuration.YandexCloudLockbox;

/// <summary>
/// Поставщик токенов доступа с помощью JWT-токена.
/// </summary>
public class JwtCredentialsProvider : ICredentialsProvider
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="host">Хост и порт, к которому производится подключение.</param>
    /// <param name="jwtToken">JWT-токен, который необходимо использовать для получения IAM-токена.</param>
    public JwtCredentialsProvider(string host, string jwtToken)
    {
        _tokenService = GetIamTokenServiceClient(host);
        _jwtToken = jwtToken;
    }

    private readonly IamTokenService.IamTokenServiceClient _tokenService;
    private readonly string _jwtToken;
    private CreateIamTokenResponse? _iamToken;

    private static IamTokenService.IamTokenServiceClient GetIamTokenServiceClient(string host)
    {
        Channel channel = new Channel(host, new SslCredentials());
        return new IamTokenService.IamTokenServiceClient(channel);
    }

    /// <inheritdoc />
    public string GetToken()
    {
        long expiration = DateTimeOffset.Now.ToUnixTimeSeconds() + 300;

        if (_iamToken == null || _iamToken.ExpiresAt.Seconds > expiration)
        {
            _iamToken = _tokenService.Create(new CreateIamTokenRequest()
            {
                Jwt = _jwtToken
            });
        }

        return _iamToken.IamToken;
    }
}
