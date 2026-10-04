using TwoGether.Domain.Common;

namespace TwoGether.Domain.Auth;

public sealed class RefreshToken : AggregateRoot
{
    private RefreshToken() { }

    public Guid UserId { get; private init; }
    public string TokenHash { get; private init; } = null!;
    public DateTimeOffset ExpiresAt { get; private init; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public static RefreshToken Issue(Guid userId, string tokenHash, DateTimeOffset expiresAt) =>
        new() { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt };

    public bool IsRevoked => RevokedAt is not null;

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    public void Revoke(DateTimeOffset now) => RevokedAt ??= now;
}
