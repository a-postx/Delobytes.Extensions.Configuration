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
    /// <param name="jwtToken">JWT-токен, который необходимо использовать для получения IAM-токена.</param>
    public JwtCredentialsProvider(string jwtToken)
    {
        _tokenService = TokenService();
        _jwtToken = jwtToken;
    }

    private readonly IamTokenService.IamTokenServiceClient _tokenService;
    private readonly string _jwtToken;
    private CreateIamTokenResponse _iamToken;

    private IamTokenService.IamTokenServiceClient TokenService()
    {
        Channel channel = new Channel("iam.api.cloud.yandex.net:443", new SslCredentials());
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
