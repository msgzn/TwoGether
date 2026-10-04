using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Vehicles;

public sealed record VehicleDto(Guid Id, string Make, string Model, string Color, string PlateNumber, int PassengerSeats)
{
    public static VehicleDto From(Vehicle vehicle) =>
        new(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Color, vehicle.PlateNumber, vehicle.PassengerSeats);
}
