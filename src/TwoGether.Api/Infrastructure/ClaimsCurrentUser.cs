using Microsoft.IdentityModel.JsonWebTokens;
using TwoGether.Application.Abstractions;

namespace TwoGether.Api.Infrastructure;

internal sealed class ClaimsCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The request is not authenticated.");
}
