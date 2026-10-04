using TwoGether.Application.Abstractions;

namespace TwoGether.Api.Infrastructure;

/// <summary>
/// Temporary stand-in for real authentication: trusts the <c>X-User-Id</c> header.
/// Replace with a claims-based implementation once auth is in place — nothing else needs to change.
/// </summary>
internal sealed class HeaderCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public const string HeaderName = "X-User-Id";

    public Guid UserId =>
        Guid.TryParse(accessor.HttpContext?.Request.Headers[HeaderName], out var userId)
            ? userId
            : throw new UnauthorizedAccessException($"Missing or invalid '{HeaderName}' header.");
}
