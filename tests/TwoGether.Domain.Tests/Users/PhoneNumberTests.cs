using TwoGether.Domain.Common;
using TwoGether.Domain.Users;

namespace TwoGether.Domain.Tests.Users;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+7 701 123 45 67")]
    [InlineData("87011234567")]
    [InlineData("7(701)123-45-67")]
    public void Normalizes_to_e164(string input)
    {
        PhoneNumber.Create(input).Value.Should().Be("+77011234567");
    }

    [Theory]
    [InlineData("")]
    [InlineData("+1 202 555 0100")]
    [InlineData("701123")]
    public void Rejects_non_kazakhstan_numbers(string input)
    {
        var act = () => PhoneNumber.Create(input);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("user.invalid_phone");
    }
}
