using TwoGether.Domain.Users;

namespace TwoGether.Application.Abstractions;

public interface ISmsSender
{
    Task SendAsync(PhoneNumber phoneNumber, string message, CancellationToken ct);
}
