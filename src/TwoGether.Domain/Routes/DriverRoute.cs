using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Domain.Routes;

public sealed class DriverRoute : AggregateRoot
{
    private DriverRoute() { }

    public Guid DriverId { get; private init; }
    public Guid VehicleId { get; private init; }
    public Point Origin { get; private init; } = null!;
    public Point Destination { get; private init; } = null!;
    public LineString Path { get; private init; } = null!;
    public double DistanceMeters { get; private init; }
    public double DurationSeconds { get; private init; }
    public RideDays Days { get; private init; }
    public TimeOnly DepartureTime { get; private init; }
    public int Seats { get; private init; }
    public bool IsActive { get; private set; }

    public static DriverRoute Create(
        Guid driverId,
        Vehicle vehicle,
        Point origin,
        Point destination,
        RoutePath path,
        RideDays days,
        TimeOnly departureTime,
        int? seats)
    {
        if (!vehicle.IsOwnedBy(driverId))
            throw new DomainException("route.vehicle_not_owned", "You can only use your own vehicle.");

        RouteRules.EnsureValidEnds(origin, destination);
        RouteRules.EnsureValidDays(days);

        var offeredSeats = seats ?? vehicle.PassengerSeats;
        if (offeredSeats < 1 || offeredSeats > vehicle.PassengerSeats)
            throw new DomainException("route.invalid_seats", $"Seats must be between 1 and {vehicle.PassengerSeats} for this vehicle.");

        return new DriverRoute
        {
            DriverId = driverId,
            VehicleId = vehicle.Id,
            Origin = origin,
            Destination = destination,
            Path = path.Line,
            DistanceMeters = path.DistanceMeters,
            DurationSeconds = path.DurationSeconds,
            Days = days,
            DepartureTime = departureTime,
            Seats = offeredSeats,
            IsActive = true,
        };
    }

    public TimeOnly TimeAt(double fractionAlongPath) =>
        DepartureTime.Add(TimeSpan.FromMinutes(Math.Round(DurationSeconds * Math.Clamp(fractionAlongPath, 0, 1) / 60, MidpointRounding.AwayFromZero)));

    public void Pause() => IsActive = false;

    public void Resume() => IsActive = true;
}
