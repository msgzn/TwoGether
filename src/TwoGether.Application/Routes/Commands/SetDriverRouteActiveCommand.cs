using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes.Commands;

public class SetDriverRouteActiveCommand : ICommand<DriverRouteDto>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class SetDriverRouteActiveCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    RouteMatcher matcher) : ICommandHandler<SetDriverRouteActiveCommand, DriverRouteDto>
{
    public async ValueTask<DriverRouteDto> Handle(SetDriverRouteActiveCommand command, CancellationToken ct)
    {
        var route = await db.DriverRoutes.SingleOrDefaultAsync(r => r.Id == command.Id && r.DriverId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(DriverRoute), command.Id);

        if (command.IsActive)
            route.Resume();
        else
            route.Pause();

        await unitOfWork.SaveChangesAsync(ct);
        await matcher.RecomputeAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return DriverRouteDto.From(route);
    }
}
