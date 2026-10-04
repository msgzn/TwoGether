using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Routes;

public static class ServiceArea
{
    public const double MinLatitude = 43.05;
    public const double MaxLatitude = 43.45;
    public const double MinLongitude = 76.65;
    public const double MaxLongitude = 77.25;

    public static void EnsureContains(Point point)
    {
        if (point.Y is < MinLatitude or > MaxLatitude || point.X is < MinLongitude or > MaxLongitude)
            throw new DomainException("route.outside_service_area", "Only trips within Almaty are supported.");
    }
}
