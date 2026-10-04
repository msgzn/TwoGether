using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TwoGether.Application.Abstractions;

namespace TwoGether.Infrastructure.Auth;

internal sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public TimeSpan RefreshTokenLifetime => _options.RefreshTokenLifetime;

    public AccessToken CreateAccessToken(Guid userId, DateTimeOffset now)
    {
        var expiresAt = now + _options.AccessTokenLifetime;

        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())]),
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(_options.CreateSigningKey(), SecurityAlgorithms.HmacSha256),
        });

        return new AccessToken(token, expiresAt);
    }
}
