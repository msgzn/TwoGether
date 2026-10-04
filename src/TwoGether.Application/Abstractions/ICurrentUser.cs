namespace TwoGether.Application.Abstractions;

public interface ICurrentUser
{
    /// <exception cref="UnauthorizedAccessException">The request is not authenticated.</exception>
    Guid UserId { get; }
}
