using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes.Commands;

public class DeleteDriverRouteCommand : ICommand
{
    public Guid Id { get; set; }
}

public class DeleteDriverRouteCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    RouteMatcher matcher) : ICommandHandler<DeleteDriverRouteCommand>
{
    public async ValueTask<Unit> Handle(DeleteDriverRouteCommand command, CancellationToken ct)
    {
        var route = await db.DriverRoutes.SingleOrDefaultAsync(r => r.Id == command.Id && r.DriverId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(DriverRoute), command.Id);

        route.SoftDelete();
        await matcher.RemoveAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
