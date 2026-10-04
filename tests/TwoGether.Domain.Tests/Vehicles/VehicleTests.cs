using TwoGether.Domain.Common;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Domain.Tests.Vehicles;

public class VehicleTests
{
    private static readonly Guid Owner = Guid.NewGuid();

    [Fact]
    public void Plate_is_normalized()
    {
        var vehicle = Vehicle.Register(Owner, "Toyota", "Camry", "Белый", " 123 abc-02 ", 3);

        vehicle.PlateNumber.Should().Be("123ABC02");
        vehicle.IsOwnedBy(Owner).Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void Seats_out_of_range_are_rejected(int seats)
    {
        var act = () => Vehicle.Register(Owner, "Toyota", "Camry", "Белый", "123ABC02", seats);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("vehicle.invalid_seats");
    }

    [Fact]
    public void Invalid_plate_is_rejected()
    {
        var act = () => Vehicle.Register(Owner, "Toyota", "Camry", "Белый", "№1", 3);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("vehicle.invalid_plate");
    }
}
