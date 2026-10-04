using TwoGether.Domain.Common;
using TwoGether.Domain.Users;

namespace TwoGether.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Registered_user_has_incomplete_profile_until_names_are_set()
    {
        var user = User.Register(PhoneNumber.Create("+77011234567"));
        user.IsProfileCompleted.Should().BeFalse();

        user.UpdateProfile("  Айгерим ", "Сапарова");

        user.IsProfileCompleted.Should().BeTrue();
        user.FirstName.Should().Be("Айгерим");
    }

    [Fact]
    public void Blank_name_is_rejected()
    {
        var user = User.Register(PhoneNumber.Create("+77011234567"));

        var act = () => user.UpdateProfile(" ", "Сапарова");

        act.Should().Throw<DomainException>().Which.Code.Should().Be("user.first_name_required");
    }
}
