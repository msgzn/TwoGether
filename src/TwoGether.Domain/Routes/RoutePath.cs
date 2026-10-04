using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Routes;

public sealed record RoutePath
{
    public RoutePath(LineString line, double distanceMeters, double durationSeconds)
    {
        if (line.NumPoints < 2 || distanceMeters <= 0 || durationSeconds <= 0)
            throw new DomainException("route.invalid_path", "Route path is empty.");

        line.SRID = Geo.Wgs84Srid;
        Line = line;
        DistanceMeters = distanceMeters;
        DurationSeconds = durationSeconds;
    }

    public LineString Line { get; }
    public double DistanceMeters { get; }
    public double DurationSeconds { get; }
}
