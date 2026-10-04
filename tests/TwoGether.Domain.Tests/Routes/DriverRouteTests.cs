using NetTopologySuite.Geometries;
using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Domain.Tests.Routes;

public class DriverRouteTests
{
    private static readonly Guid Driver = Guid.NewGuid();
    private static readonly Point Sairan = Geo.CreatePoint(43.2390, 76.8297);
    private static readonly Point Dostyk = Geo.CreatePoint(43.2389, 76.9455);
    private static readonly Vehicle Car = Vehicle.Register(Driver, "Toyota", "Camry", "Белый", "123ABC02", 3);

    private static RoutePath Path() =>
        new(new LineString([Sairan.Coordinate, Dostyk.Coordinate]), distanceMeters: 11800, durationSeconds: 900);

    private static DriverRoute Create(Vehicle? vehicle = null, int? seats = null, Point? destination = null) =>
        DriverRoute.Create(Driver, vehicle ?? Car, Sairan, destination ?? Dostyk, Path(), RideDays.Weekdays, new TimeOnly(8, 0), seats);

    [Fact]
    public void Seats_default_to_vehicle_capacity()
    {
        Create().Seats.Should().Be(3);
    }

    [Fact]
    public void Cannot_offer_more_seats_than_vehicle_has()
    {
        var act = () => Create(seats: 4);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.invalid_seats");
    }

    [Fact]
    public void Cannot_use_someone_elses_vehicle()
    {
        var foreignCar = Vehicle.Register(Guid.NewGuid(), "Kia", "Rio", "Серый", "777AAA02", 4);

        var act = () => Create(vehicle: foreignCar);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.vehicle_not_owned");
    }

    [Fact]
    public void Trips_outside_almaty_are_rejected()
    {
        var astana = Geo.CreatePoint(51.1694, 71.4491);

        var act = () => Create(destination: astana);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.outside_service_area");
    }

    [Fact]
    public void Time_at_point_is_interpolated_along_the_path()
    {
        var route = Create();

        route.TimeAt(0).Should().Be(new TimeOnly(8, 0));
        route.TimeAt(0.5).Should().Be(new TimeOnly(8, 8));
        route.TimeAt(1).Should().Be(new TimeOnly(8, 15));
    }
}
