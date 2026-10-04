using TwoGether.Application.Routes;

namespace TwoGether.Application.Matches;

public enum MatchRole
{
    Driver,
    Passenger,
}

public sealed record MatchDto(
    Guid Id,
    MatchRole MyRole,
    Guid DriverRouteId,
    Guid PassengerRouteId,
    string? CounterpartFirstName,
    MatchVehicleDto Vehicle,
    IReadOnlyList<DayOfWeek> Days,
    Location PickupPoint,
    Location DropoffPoint,
    TimeOnly PickupTime,
    double WalkToPickupMeters,
    double WalkFromDropoffMeters,
    double SharedDistanceMeters,
    decimal Price);

public sealed record MatchVehicleDto(string Make, string Model, string Color);
