using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Routes;

internal static class RouteRules
{
    public const double MinTripMeters = 500;

    public static void EnsureValidEnds(Point origin, Point destination)
    {
        ServiceArea.EnsureContains(origin);
        ServiceArea.EnsureContains(destination);

        if (Geo.DistanceMeters(origin, destination) < MinTripMeters)
            throw new DomainException("route.too_short", $"Origin and destination must be at least {MinTripMeters:0} m apart.");
    }

    public static void EnsureValidDays(RideDays days)
    {
        if (days == RideDays.None || (days & ~RideDays.All) != 0)
            throw new DomainException("route.invalid_days", "Choose at least one day of the week.");
    }
}
