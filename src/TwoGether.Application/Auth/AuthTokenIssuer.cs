using TwoGether.Application.Abstractions;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Users;

namespace TwoGether.Application.Auth;

public sealed class AuthTokenIssuer(IAppDbContext db, ITokenService tokens)
{
    public AuthResponse Issue(User user, DateTimeOffset now)
    {
        var accessToken = tokens.CreateAccessToken(user.Id, now);

        var refreshToken = Secrets.GenerateRefreshToken();
        var refreshTokenExpiresAt = now + tokens.RefreshTokenLifetime;
        db.RefreshTokens.Add(RefreshToken.Issue(user.Id, Secrets.Hash(refreshToken), refreshTokenExpiresAt));

        return new AuthResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            refreshToken,
            refreshTokenExpiresAt,
            user.IsProfileCompleted);
    }
}
