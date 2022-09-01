using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.IO;

namespace Delobytes.Extensions.Configuration.YandexCloudLockbox;

internal class JwtTokenGenerator
{
    public JwtTokenGenerator(string serviceAccountId, string serviceAccountAuthorizedKeyId, string privateKey, string jwtTokenAudience)
    {
        if (string.IsNullOrEmpty(serviceAccountId))
        {
            throw new ArgumentException("Service account ID is not found.", nameof(serviceAccountId));
        }

        if (string.IsNullOrEmpty(serviceAccountAuthorizedKeyId))
        {
            throw new ArgumentException("Service account Authorized key ID is not found.", nameof(serviceAccountAuthorizedKeyId));
        }

        if (string.IsNullOrEmpty(privateKey))
        {
            throw new ArgumentException("Private key path is not found.", nameof(privateKey));
        }

        _serviceAccountId = serviceAccountId;
        _serviceAccountAuthorizedKeyId = serviceAccountAuthorizedKeyId;
        _privateKey = privateKey;
        _jwtTokenAudience = jwtTokenAudience;
    }

    private readonly string _serviceAccountId;
    private readonly string _serviceAccountAuthorizedKeyId;
    private readonly string _privateKey;
    private readonly string _jwtTokenAudience; 

    internal string GetEncodedJwtToken()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Dictionary<string, object> headers = new Dictionary<string, object>() { { "kid", _serviceAccountAuthorizedKeyId } };

        Dictionary<string, object> payload = new Dictionary<string, object>()
        {
            { "aud", _jwtTokenAudience },
            { "iss", _serviceAccountId },
            { "iat", now },
            { "exp", now + 3600 }
        };

        RsaPrivateCrtKeyParameters? privateKeyParams;

        using (TextReader pemReader = new StringReader(_privateKey))
        {
            privateKeyParams = new PemReader(pemReader).ReadObject() as RsaPrivateCrtKeyParameters;
        }

        if (privateKeyParams == null)
        {
            throw new InvalidOperationException("RSA private key params is not available");
        }

#pragma warning disable CA1416 // Platform compatibility checked
        RSA rsa;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rsa = new RSACng();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            rsa = new RSAOpenSsl();
        }
        else
        {
            throw new InvalidOperationException("MacOS is not supported");
        }
#pragma warning restore CA1416 // Validate platform compatibility

        rsa.ImportParameters(DotNetUtilities.ToRSAParameters(privateKeyParams));
        string encodedToken = JWT.Encode(payload, rsa, JwsAlgorithm.PS256, headers);

        return encodedToken;
    }
}
