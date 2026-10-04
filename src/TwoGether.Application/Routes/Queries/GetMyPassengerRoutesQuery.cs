using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Application.Routes.Queries;

public class GetMyPassengerRoutesQuery : IQuery<IReadOnlyList<PassengerRouteDto>>;

public class GetMyPassengerRoutesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    : IQueryHandler<GetMyPassengerRoutesQuery, IReadOnlyList<PassengerRouteDto>>
{
    public async ValueTask<IReadOnlyList<PassengerRouteDto>> Handle(GetMyPassengerRoutesQuery query, CancellationToken ct)
    {
        var routes = await db.PassengerRoutes
            .AsNoTracking()
            .Where(r => r.PassengerId == currentUser.UserId)
            .OrderBy(r => r.EarliestPickup)
            .ToListAsync(ct);

        return routes.Select(PassengerRouteDto.From).ToList();
    }
}
