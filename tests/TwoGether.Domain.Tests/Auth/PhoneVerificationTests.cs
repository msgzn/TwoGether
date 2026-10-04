using TwoGether.Domain.Auth;
using TwoGether.Domain.Common;
using TwoGether.Domain.Users;

namespace TwoGether.Domain.Tests.Auth;

public class PhoneVerificationTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly PhoneNumber Phone = PhoneNumber.Create("+77011234567");

    private static PhoneVerification Start() => PhoneVerification.Start(Phone, "right", Now, previous: null);

    [Fact]
    public void Correct_code_confirms()
    {
        var verification = Start();

        verification.TryConfirm("right", Now.AddMinutes(1)).Should().BeTrue();
        verification.ConfirmedAt.Should().NotBeNull();
    }

    [Fact]
    public void Code_cannot_be_used_twice()
    {
        var verification = Start();
        verification.TryConfirm("right", Now);

        var act = () => verification.TryConfirm("right", Now);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("auth.code_already_used");
    }

    [Fact]
    public void Wrong_code_counts_attempts_until_locked()
    {
        var verification = Start();
        for (var i = 0; i < PhoneVerification.MaxAttempts; i++)
            verification.TryConfirm("wrong", Now).Should().BeFalse();

        var act = () => verification.TryConfirm("right", Now);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("auth.too_many_attempts");
    }

    [Fact]
    public void Expired_code_is_rejected()
    {
        var verification = Start();

        var act = () => verification.TryConfirm("right", Now + PhoneVerification.Lifetime);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("auth.code_expired");
    }

    [Fact]
    public void New_code_cannot_be_requested_during_cooldown()
    {
        var previous = Start();

        var act = () => PhoneVerification.Start(Phone, "next", Now.AddSeconds(30), previous);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("auth.resend_too_soon");
        PhoneVerification.Start(Phone, "next", Now + PhoneVerification.ResendCooldown, previous).Should().NotBeNull();
    }
}
