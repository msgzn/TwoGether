using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Application.Vehicles.Queries;

public class GetMyVehiclesQuery : IQuery<IReadOnlyList<VehicleDto>>;

public class GetMyVehiclesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    : IQueryHandler<GetMyVehiclesQuery, IReadOnlyList<VehicleDto>>
{
    public async ValueTask<IReadOnlyList<VehicleDto>> Handle(GetMyVehiclesQuery query, CancellationToken ct) =>
        await db.Vehicles
            .AsNoTracking()
            .Where(v => v.OwnerId == currentUser.UserId)
            .OrderBy(v => v.InsertedAt)
            .Select(v => new VehicleDto(v.Id, v.Make, v.Model, v.Color, v.PlateNumber, v.PassengerSeats))
            .ToListAsync(ct);
}
