using TwoGether.Domain.Common;
using TwoGether.Domain.Users;

namespace TwoGether.Domain.Auth;

public sealed class PhoneVerification : AggregateRoot
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);
    public const int MaxAttempts = 5;

    private PhoneVerification() { }

    public PhoneNumber PhoneNumber { get; private init; } = null!;
    public string CodeHash { get; private init; } = null!;
    public DateTimeOffset SentAt { get; private init; }
    public DateTimeOffset ExpiresAt { get; private init; }
    public int Attempts { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }

    public static PhoneVerification Start(PhoneNumber phoneNumber, string codeHash, DateTimeOffset now, PhoneVerification? previous)
    {
        if (previous is not null && now < previous.SentAt + ResendCooldown)
            throw new DomainException("auth.resend_too_soon", $"A new code can be requested {ResendCooldown.TotalSeconds:0} seconds after the previous one.");

        return new PhoneVerification
        {
            PhoneNumber = phoneNumber,
            CodeHash = codeHash,
            SentAt = now,
            ExpiresAt = now + Lifetime,
        };
    }

    public bool TryConfirm(string codeHash, DateTimeOffset now)
    {
        if (ConfirmedAt is not null)
            throw new DomainException("auth.code_already_used", "This code has already been used. Request a new one.");
        if (now >= ExpiresAt)
            throw new DomainException("auth.code_expired", "The code has expired. Request a new one.");
        if (Attempts >= MaxAttempts)
            throw new DomainException("auth.too_many_attempts", "Too many wrong attempts. Request a new code.");

        Attempts++;
        if (!string.Equals(CodeHash, codeHash, StringComparison.Ordinal))
            return false;

        ConfirmedAt = now;
        return true;
    }
}
