using TwoGether.Domain.Common;

namespace TwoGether.Domain.Tests;

public class GeoTests
{
    [Fact]
    public void CreatePoint_puts_longitude_in_x_and_latitude_in_y()
    {
        var point = Geo.CreatePoint(latitude: 43.2389, longitude: 76.8897);

        point.X.Should().Be(76.8897);
        point.Y.Should().Be(43.2389);
        point.SRID.Should().Be(Geo.Wgs84Srid);
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(0, 181)]
    public void CreatePoint_rejects_out_of_range_coordinates(double latitude, double longitude)
    {
        var act = () => Geo.CreatePoint(latitude, longitude);

        act.Should().Throw<DomainException>();
    }
}
