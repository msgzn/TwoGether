using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Vehicles.Commands;

public class CreateVehicleCommand : VehicleInput, ICommand<VehicleDto>;

public class CreateVehicleCommandValidator : VehicleInputValidator<CreateVehicleCommand>;

public class CreateVehicleCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<CreateVehicleCommand, VehicleDto>
{
    public async ValueTask<VehicleDto> Handle(CreateVehicleCommand command, CancellationToken ct)
    {
        var plate = Vehicle.NormalizePlate(command.PlateNumber);
        if (await db.Vehicles.AnyAsync(v => v.PlateNumber == plate, ct))
            throw new DomainException("vehicle.plate_taken", "A vehicle with this plate number is already registered.");

        var vehicle = Vehicle.Register(
            currentUser.UserId, command.Make, command.Model, command.Color, plate, command.PassengerSeats);

        db.Vehicles.Add(vehicle);
        await unitOfWork.SaveChangesAsync(ct);

        return VehicleDto.From(vehicle);
    }
}
