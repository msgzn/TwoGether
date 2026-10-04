namespace TwoGether.Application.Abstractions;

public interface ITokenService
{
    TimeSpan RefreshTokenLifetime { get; }

    AccessToken CreateAccessToken(Guid userId, DateTimeOffset now);
}

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);
