using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes.Commands;

public class DeletePassengerRouteCommand : ICommand
{
    public Guid Id { get; set; }
}

public class DeletePassengerRouteCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    RouteMatcher matcher) : ICommandHandler<DeletePassengerRouteCommand>
{
    public async ValueTask<Unit> Handle(DeletePassengerRouteCommand command, CancellationToken ct)
    {
        var route = await db.PassengerRoutes.SingleOrDefaultAsync(r => r.Id == command.Id && r.PassengerId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(PassengerRoute), command.Id);

        route.SoftDelete();
        await matcher.RemoveAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
