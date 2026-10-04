using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Matches;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Matches;

public sealed class RouteMatcher(IAppDbContext db, IOptions<MatchingOptions> options)
{
    private readonly MatchingOptions _options = options.Value;

    public async Task RecomputeAsync(DriverRoute route, CancellationToken ct)
    {
        await RemoveAsync(route, ct);
        if (!route.IsActive)
            return;

        var candidates = await MatchCandidates().Where(c => c.DriverRouteId == route.Id).ToListAsync(ct);
        var passengerRouteIds = candidates.Select(c => c.PassengerRouteId).ToList();
        var passengerRoutes = await db.PassengerRoutes.Where(p => passengerRouteIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        AddMatches(candidates, c => route, c => passengerRoutes[c.PassengerRouteId]);
    }

    public async Task RecomputeAsync(PassengerRoute route, CancellationToken ct)
    {
        await RemoveAsync(route, ct);
        if (!route.IsActive)
            return;

        var candidates = await MatchCandidates().Where(c => c.PassengerRouteId == route.Id).ToListAsync(ct);
        var driverRouteIds = candidates.Select(c => c.DriverRouteId).ToList();
        var driverRoutes =
            await db.DriverRoutes.Where(d => driverRouteIds.Contains(d.Id)).ToDictionaryAsync(d => d.Id, ct);

        AddMatches(candidates, c => driverRoutes[c.DriverRouteId], c => route);
    }

    public Task RemoveAsync(DriverRoute route, CancellationToken ct) =>
        db.RouteMatches.Where(m => m.DriverRouteId == route.Id).ExecuteDeleteAsync(ct);

    public Task RemoveAsync(PassengerRoute route, CancellationToken ct) =>
        db.RouteMatches.Where(m => m.PassengerRouteId == route.Id).ExecuteDeleteAsync(ct);

    private void AddMatches(
        IEnumerable<MatchCandidate> candidates,
        Func<MatchCandidate, DriverRoute> driverRouteOf,
        Func<MatchCandidate, PassengerRoute> passengerRouteOf)
    {
        foreach (var candidate in candidates)
        {
            var driverRoute = driverRouteOf(candidate);
            var passengerRoute = passengerRouteOf(candidate);

            var pickupTime = driverRoute.TimeAt(candidate.PickupFraction);
            if (!passengerRoute.AcceptsPickupAt(pickupTime))
                continue;

            var pickupPoint = PointAlong(driverRoute.Path, candidate.PickupFraction);
            var dropoffPoint = PointAlong(driverRoute.Path, candidate.DropoffFraction);
            var sharedDistance = (candidate.DropoffFraction - candidate.PickupFraction) * driverRoute.DistanceMeters;

            db.RouteMatches.Add(RouteMatch.Create(
                driverRoute,
                passengerRoute,
                pickupPoint,
                dropoffPoint,
                pickupTime,
                candidate.WalkToPickupMeters,
                candidate.WalkFromDropoffMeters,
                sharedDistance,
                _options.CalculatePrice(sharedDistance)));
        }
    }

    private static Point PointAlong(LineString path, double fraction)
    {
        var coordinate = new LengthIndexedLine(path).ExtractPoint(fraction * path.Length);
        return Geo.CreatePoint(coordinate.Y, coordinate.X);
    }

    private IQueryable<MatchCandidate> MatchCandidates() =>
        db.Database.SqlQuery<MatchCandidate>($"""
                                              SELECT
                                                  d.id AS driver_route_id,
                                                  p.id AS passenger_route_id,
                                                  ST_LineLocatePoint(d.path, p.origin) AS pickup_fraction,
                                                  ST_LineLocatePoint(d.path, p.destination) AS dropoff_fraction,
                                                  ST_Distance(p.origin::geography, d.path::geography) AS walk_to_pickup_meters,
                                                  ST_Distance(p.destination::geography, d.path::geography) AS walk_from_dropoff_meters
                                              FROM driver_routes d
                                              JOIN passenger_routes p
                                                  ON (p.days & d.days) <> 0
                                                 AND p.passenger_id <> d.driver_id
                                                 AND ST_DWithin(p.origin::geography, d.path::geography, {_options.MaxWalkMeters})
                                                 AND ST_DWithin(p.destination::geography, d.path::geography, {_options.MaxWalkMeters})
                                              WHERE d.is_active AND NOT d.is_deleted
                                                AND p.is_active AND NOT p.is_deleted
                                                AND ST_LineLocatePoint(d.path, p.origin) < ST_LineLocatePoint(d.path, p.destination)
                                              """);

    private sealed record MatchCandidate(
        Guid DriverRouteId,
        Guid PassengerRouteId,
        double PickupFraction,
        double DropoffFraction,
        double WalkToPickupMeters,
        double WalkFromDropoffMeters);
}