using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;

namespace TwoGether.Domain.Matches;

public sealed class RouteMatch : Entity
{
    private RouteMatch() { }

    public Guid DriverRouteId { get; private init; }
    public Guid PassengerRouteId { get; private init; }
    public Guid DriverId { get; private init; }
    public Guid PassengerId { get; private init; }
    public RideDays CommonDays { get; private init; }
    public Point PickupPoint { get; private init; } = null!;
    public Point DropoffPoint { get; private init; } = null!;
    public TimeOnly PickupTime { get; private init; }
    public double WalkToPickupMeters { get; private init; }
    public double WalkFromDropoffMeters { get; private init; }
    public double SharedDistanceMeters { get; private init; }
    public decimal Price { get; private init; }

    public static RouteMatch Create(
        DriverRoute driverRoute,
        PassengerRoute passengerRoute,
        Point pickupPoint,
        Point dropoffPoint,
        TimeOnly pickupTime,
        double walkToPickupMeters,
        double walkFromDropoffMeters,
        double sharedDistanceMeters,
        decimal price) => new()
    {
        DriverRouteId = driverRoute.Id,
        PassengerRouteId = passengerRoute.Id,
        DriverId = driverRoute.DriverId,
        PassengerId = passengerRoute.PassengerId,
        CommonDays = driverRoute.Days & passengerRoute.Days,
        PickupPoint = pickupPoint,
        DropoffPoint = dropoffPoint,
        PickupTime = pickupTime,
        WalkToPickupMeters = walkToPickupMeters,
        WalkFromDropoffMeters = walkFromDropoffMeters,
        SharedDistanceMeters = sharedDistanceMeters,
        Price = price,
    };
}
