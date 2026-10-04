using TwoGether.Application.Abstractions;

namespace TwoGether.Api.Infrastructure;

internal sealed class HeaderCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public const string HeaderName = "X-User-Id";

    public Guid UserId =>
        Guid.TryParse(accessor.HttpContext?.Request.Headers[HeaderName], out var userId)
            ? userId
            : throw new UnauthorizedAccessException($"Missing or invalid '{HeaderName}' header.");
}
