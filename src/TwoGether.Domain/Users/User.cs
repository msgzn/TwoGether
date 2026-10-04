using TwoGether.Domain.Common;

namespace TwoGether.Domain.Users;

public sealed class User : AggregateRoot
{
    public const int NameMaxLength = 50;

    private User() { }

    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }

    public bool IsProfileCompleted => FirstName is not null && LastName is not null;

    public static User Register(PhoneNumber phoneNumber) => new() { PhoneNumber = phoneNumber };

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = NormalizeName(firstName, "user.first_name_required");
        LastName = NormalizeName(lastName, "user.last_name_required");
    }

    private static string NormalizeName(string name, string errorCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(errorCode, "Name is required.");

        name = name.Trim();
        if (name.Length > NameMaxLength)
            throw new DomainException("user.name_too_long", $"Name must be at most {NameMaxLength} characters.");

        return name;
    }
}
