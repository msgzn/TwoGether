using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes.Commands;

public class SetPassengerRouteActiveCommand : ICommand<PassengerRouteDto>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class SetPassengerRouteActiveCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    RouteMatcher matcher) : ICommandHandler<SetPassengerRouteActiveCommand, PassengerRouteDto>
{
    public async ValueTask<PassengerRouteDto> Handle(SetPassengerRouteActiveCommand command, CancellationToken ct)
    {
        var route = await db.PassengerRoutes.SingleOrDefaultAsync(r => r.Id == command.Id && r.PassengerId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(PassengerRoute), command.Id);

        if (command.IsActive)
            route.Resume();
        else
            route.Pause();

        await unitOfWork.SaveChangesAsync(ct);
        await matcher.RecomputeAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return PassengerRouteDto.From(route);
    }
}
