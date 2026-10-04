using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Routes;

namespace TwoGether.Application.Matches.Queries;

public class GetMyMatchesQuery : IQuery<IReadOnlyList<MatchDto>>;

public class GetMyMatchesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    : IQueryHandler<GetMyMatchesQuery, IReadOnlyList<MatchDto>>
{
    public async ValueTask<IReadOnlyList<MatchDto>> Handle(GetMyMatchesQuery query, CancellationToken ct)
    {
        var me = currentUser.UserId;

        var rows = await (
                from match in db.RouteMatches.AsNoTracking()
                where match.DriverId == me || match.PassengerId == me
                join driver in db.Users on match.DriverId equals driver.Id
                join passenger in db.Users on match.PassengerId equals passenger.Id
                join driverRoute in db.DriverRoutes on match.DriverRouteId equals driverRoute.Id
                join vehicle in db.Vehicles.IgnoreQueryFilters([QueryFilters.SoftDelete]) on driverRoute.VehicleId equals vehicle.Id
                orderby match.PickupTime
                select new
                {
                    Match = match,
                    DriverFirstName = driver.FirstName,
                    PassengerFirstName = passenger.FirstName,
                    vehicle.Make,
                    vehicle.Model,
                    vehicle.Color,
                })
            .ToListAsync(ct);

        return rows.Select(r =>
        {
            var role = r.Match.DriverId == me ? MatchRole.Driver : MatchRole.Passenger;
            return new MatchDto(
                r.Match.Id,
                role,
                r.Match.DriverRouteId,
                r.Match.PassengerRouteId,
                role == MatchRole.Driver ? r.PassengerFirstName : r.DriverFirstName,
                new MatchVehicleDto(r.Make, r.Model, r.Color),
                RideDaysMapper.ToDays(r.Match.CommonDays),
                Location.From(r.Match.PickupPoint),
                Location.From(r.Match.DropoffPoint),
                r.Match.PickupTime,
                Math.Round(r.Match.WalkToPickupMeters),
                Math.Round(r.Match.WalkFromDropoffMeters),
                Math.Round(r.Match.SharedDistanceMeters),
                r.Match.Price);
        }).ToList();
    }
}
