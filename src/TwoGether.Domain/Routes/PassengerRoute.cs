using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Routes;

public sealed class PassengerRoute : AggregateRoot
{
    public static readonly TimeSpan MaxPickupWindow = TimeSpan.FromHours(3);

    private PassengerRoute() { }

    public Guid PassengerId { get; private init; }
    public Point Origin { get; private init; } = null!;
    public Point Destination { get; private init; } = null!;
    public RideDays Days { get; private init; }
    public TimeOnly EarliestPickup { get; private init; }
    public TimeOnly LatestPickup { get; private init; }
    public bool IsActive { get; private set; }

    public static PassengerRoute Create(
        Guid passengerId,
        Point origin,
        Point destination,
        RideDays days,
        TimeOnly earliestPickup,
        TimeOnly latestPickup)
    {
        RouteRules.EnsureValidEnds(origin, destination);
        RouteRules.EnsureValidDays(days);

        if (latestPickup < earliestPickup)
            throw new DomainException("route.invalid_pickup_window", "Latest pickup time must not be earlier than the earliest one.");
        if (latestPickup - earliestPickup > MaxPickupWindow)
            throw new DomainException("route.pickup_window_too_wide", $"Pickup window must be at most {MaxPickupWindow.TotalHours:0} hours.");

        return new PassengerRoute
        {
            PassengerId = passengerId,
            Origin = origin,
            Destination = destination,
            Days = days,
            EarliestPickup = earliestPickup,
            LatestPickup = latestPickup,
            IsActive = true,
        };
    }

    public bool AcceptsPickupAt(TimeOnly time) => time >= EarliestPickup && time <= LatestPickup;

    public void Pause() => IsActive = false;

    public void Resume() => IsActive = true;
}
