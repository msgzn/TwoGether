using FluentValidation;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Vehicles;

public abstract class VehicleInput
{
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public string Color { get; set; } = "";
    public string PlateNumber { get; set; } = "";
    public int PassengerSeats { get; set; }
}

public abstract class VehicleInputValidator<T> : AbstractValidator<T> where T : VehicleInput
{
    protected VehicleInputValidator()
    {
        RuleFor(v => v.Make).NotEmpty().MaximumLength(Vehicle.TextMaxLength);
        RuleFor(v => v.Model).NotEmpty().MaximumLength(Vehicle.TextMaxLength);
        RuleFor(v => v.Color).NotEmpty().MaximumLength(Vehicle.TextMaxLength);
        RuleFor(v => v.PlateNumber).NotEmpty();
        RuleFor(v => v.PassengerSeats).InclusiveBetween(Vehicle.MinPassengerSeats, Vehicle.MaxPassengerSeats);
    }
}
