using System.Text.RegularExpressions;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Users;

public sealed partial record PhoneNumber
{
    private PhoneNumber(string value) => Value = value;

    public string Value { get; }

    public static PhoneNumber Create(string input)
    {
        var digits = NonDigits().Replace(input ?? "", "");

        if (digits.Length == 11 && digits[0] == '8')
            digits = "7" + digits[1..];

        if (digits.Length != 11 || digits[0] != '7')
            throw new DomainException("user.invalid_phone", "Phone number must be a Kazakhstan number like +7 701 123 45 67.");

        return new PhoneNumber("+" + digits);
    }

    public static PhoneNumber FromStorage(string value) => new(value);

    public override string ToString() => Value;

    [GeneratedRegex(@"\D")]
    private static partial Regex NonDigits();
}
