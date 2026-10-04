using System.Text.Json.Serialization;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Vehicles.Commands;

public class UpdateVehicleCommand : VehicleInput, ICommand<VehicleDto>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}

public class UpdateVehicleCommandValidator : VehicleInputValidator<UpdateVehicleCommand>;

public class UpdateVehicleCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<UpdateVehicleCommand, VehicleDto>
{
    public async ValueTask<VehicleDto> Handle(UpdateVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == command.Id && v.OwnerId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), command.Id);

        var plate = Vehicle.NormalizePlate(command.PlateNumber);
        if (await db.Vehicles.AnyAsync(v => v.PlateNumber == plate && v.Id != vehicle.Id, ct))
            throw new DomainException("vehicle.plate_taken", "A vehicle with this plate number is already registered.");

        vehicle.Update(command.Make, command.Model, command.Color, plate, command.PassengerSeats);
        await unitOfWork.SaveChangesAsync(ct);

        return VehicleDto.From(vehicle);
    }
}
