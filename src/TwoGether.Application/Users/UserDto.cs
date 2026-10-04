namespace TwoGether.Application.Users;

public sealed record UserDto(Guid Id, string PhoneNumber, string? FirstName, string? LastName, bool IsProfileCompleted);
