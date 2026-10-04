using FluentValidation;
using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;

namespace TwoGether.Application.Routes;

public sealed record Location(double Latitude, double Longitude)
{
    public Point ToPoint() => Geo.CreatePoint(Latitude, Longitude);

    public static Location From(Point point) => new(point.Y, point.X);
}

public sealed class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(l => l.Latitude).InclusiveBetween(-90, 90);
        RuleFor(l => l.Longitude).InclusiveBetween(-180, 180);
    }
}
