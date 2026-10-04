using NetTopologySuite.Geometries;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Abstractions;

public interface IRoutingService
{
    Task<RoutePath> BuildRouteAsync(Point origin, Point destination, CancellationToken ct);
}
