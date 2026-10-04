namespace TwoGether.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
}
