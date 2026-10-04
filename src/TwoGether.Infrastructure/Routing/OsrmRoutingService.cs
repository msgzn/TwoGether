using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;

namespace TwoGether.Infrastructure.Routing;

internal sealed class OsrmRoutingService(HttpClient http, IOptions<OsrmOptions> options) : IRoutingService
{
    private static readonly GeometryFactory Factory = new(new PrecisionModel(), Geo.Wgs84Srid);

    public async Task<RoutePath> BuildRouteAsync(Point origin, Point destination, CancellationToken ct)
    {
        var url = string.Create(CultureInfo.InvariantCulture,
            $"{options.Value.BaseUrl.TrimEnd('/')}/route/v1/driving/{origin.X},{origin.Y};{destination.X},{destination.Y}?overview=full&geometries=geojson");

        var response = await http.GetFromJsonAsync<OsrmResponse>(url, ct)
            ?? throw new InvalidOperationException("OSRM returned an empty response.");

        if (response.Code != "Ok" || response.Routes is not [var route, ..])
            throw new DomainException("route.not_found", "No road route found between these points.");

        var coordinates = route.Geometry.Coordinates.Select(c => new Coordinate(c[0], c[1])).ToArray();
        return new RoutePath(Factory.CreateLineString(coordinates), route.Distance, route.Duration);
    }

    private sealed record OsrmResponse(string Code, List<OsrmRoute>? Routes);

    private sealed record OsrmRoute(double Distance, double Duration, OsrmGeometry Geometry);

    private sealed record OsrmGeometry(List<double[]> Coordinates);
}
