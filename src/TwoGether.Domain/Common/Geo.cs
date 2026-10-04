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
}
