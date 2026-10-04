using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Application.Routes.Queries;

public class GetMyDriverRoutesQuery : IQuery<IReadOnlyList<DriverRouteDto>>;

public class GetMyDriverRoutesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    : IQueryHandler<GetMyDriverRoutesQuery, IReadOnlyList<DriverRouteDto>>
{
    public async ValueTask<IReadOnlyList<DriverRouteDto>> Handle(GetMyDriverRoutesQuery query, CancellationToken ct)
    {
        var routes = await db.DriverRoutes
            .AsNoTracking()
            .Where(r => r.DriverId == currentUser.UserId)
            .OrderBy(r => r.DepartureTime)
            .ToListAsync(ct);

        return routes.Select(DriverRouteDto.From).ToList();
    }
}
