using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes;

public sealed record DriverRouteDto(
    Guid Id,
    Guid VehicleId,
    Location Origin,
    Location Destination,
    IReadOnlyList<Location> Path,
    double DistanceMeters,
    double DurationSeconds,
    IReadOnlyList<DayOfWeek> Days,
    TimeOnly DepartureTime,
    int Seats,
    bool IsActive)
{
    public static DriverRouteDto From(DriverRoute route) => new(
        route.Id,
        route.VehicleId,
        Location.From(route.Origin),
        Location.From(route.Destination),
        route.Path.Coordinates.Select(c => new Location(c.Y, c.X)).ToList(),
        route.DistanceMeters,
        route.DurationSeconds,
        RideDaysMapper.ToDays(route.Days),
        route.DepartureTime,
        route.Seats,
        route.IsActive);
}

public sealed record PassengerRouteDto(
    Guid Id,
    Location Origin,
    Location Destination,
    IReadOnlyList<DayOfWeek> Days,
    TimeOnly EarliestPickup,
    TimeOnly LatestPickup,
    bool IsActive)
{
    public static PassengerRouteDto From(PassengerRoute route) => new(
        route.Id,
        Location.From(route.Origin),
        Location.From(route.Destination),
        RideDaysMapper.ToDays(route.Days),
        route.EarliestPickup,
        route.LatestPickup,
        route.IsActive);
}
