using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Application.Auth.Commands;

public class LogoutCommand : ICommand
{
    public string RefreshToken { get; set; } = "";
}

public class LogoutCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : ICommandHandler<LogoutCommand>
{
    public async ValueTask<Unit> Handle(LogoutCommand command, CancellationToken ct)
    {
        var tokenHash = Secrets.Hash(command.RefreshToken);
        var token = await db.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        if (token is not null)
        {
            token.Revoke(clock.GetUtcNow());
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
