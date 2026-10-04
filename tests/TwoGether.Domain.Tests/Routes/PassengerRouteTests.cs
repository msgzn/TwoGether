using TwoGether.Domain.Common;
using TwoGether.Domain.Routes;

namespace TwoGether.Domain.Tests.Routes;

public class PassengerRouteTests
{
    private static PassengerRoute Create(TimeOnly earliest, TimeOnly latest, RideDays days = RideDays.Weekdays) =>
        PassengerRoute.Create(Guid.NewGuid(), Geo.CreatePoint(43.2400, 76.9000), Geo.CreatePoint(43.2389, 76.9455), days, earliest, latest);

    [Fact]
    public void Accepts_pickup_only_inside_window()
    {
        var route = Create(new TimeOnly(8, 0), new TimeOnly(8, 30));

        route.AcceptsPickupAt(new TimeOnly(8, 15)).Should().BeTrue();
        route.AcceptsPickupAt(new TimeOnly(8, 31)).Should().BeFalse();
    }

    [Fact]
    public void Inverted_window_is_rejected()
    {
        var act = () => Create(new TimeOnly(9, 0), new TimeOnly(8, 0));

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.invalid_pickup_window");
    }

    [Fact]
    public void At_least_one_day_is_required()
    {
        var act = () => Create(new TimeOnly(8, 0), new TimeOnly(8, 30), RideDays.None);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.invalid_days");
    }

    [Fact]
    public void Too_short_trip_is_rejected()
    {
        var act = () => PassengerRoute.Create(
            Guid.NewGuid(), Geo.CreatePoint(43.2400, 76.9000), Geo.CreatePoint(43.2401, 76.9001),
            RideDays.Monday, new TimeOnly(8, 0), new TimeOnly(8, 30));

        act.Should().Throw<DomainException>().Which.Code.Should().Be("route.too_short");
    }
}
