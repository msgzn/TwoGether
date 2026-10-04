using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Application.Auth.Commands;

public class RefreshTokenCommand : ICommand<AuthResponse>
{
    public string RefreshToken { get; set; } = "";
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(c => c.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    AuthTokenIssuer tokenIssuer,
    TimeProvider clock) : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    public async ValueTask<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var tokenHash = Secrets.Hash(command.RefreshToken);

        var token = await db.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == tokenHash, ct);
        if (token is null || token.IsExpired(now))
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");

        if (token.IsRevoked)
        {
            await RevokeAllAsync(token.UserId, now, ct);
            throw new UnauthorizedAccessException("Refresh token has already been used. Sign in again.");
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == token.UserId, ct)
            ?? throw new UnauthorizedAccessException("User no longer exists.");

        token.Revoke(now);
        var response = tokenIssuer.Issue(user, now);
        await unitOfWork.SaveChangesAsync(ct);

        return response;
    }

    private async Task RevokeAllAsync(Guid userId, DateTimeOffset now, CancellationToken ct)
    {
        var activeTokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var activeToken in activeTokens)
            activeToken.Revoke(now);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
