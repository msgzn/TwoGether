using NetTopologySuite.Geometries;

namespace TwoGether.Domain.Common;

public static class Geo
{
    public const int Wgs84Srid = 4326;
    
    public static Point CreatePoint(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new DomainException("geo.invalid_latitude", "Latitude must be between -90 and 90.");
        
        
        return longitude is < -180 or > 180 ? throw new DomainException("geo.invalid_longitude", "Longitude must be between -180 and 180.") : new Point(longitude, latitude) { SRID = Wgs84Srid };
    }

    public static double DistanceMeters(Point a, Point b)
    {
        const double earthRadiusMeters = 6_371_008.8;
        var dLat = ToRadians(b.Y - a.Y);
        var dLon = ToRadians(b.X - a.X);

        var h = Math.Pow(Math.Sin(dLat / 2), 2)
              + Math.Cos(ToRadians(a.Y)) * Math.Cos(ToRadians(b.Y)) * Math.Pow(Math.Sin(dLon / 2), 2);

        return 2 * earthRadiusMeters * Math.Asin(Math.Sqrt(h));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
