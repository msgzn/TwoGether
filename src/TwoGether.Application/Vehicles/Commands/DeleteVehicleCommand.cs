using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Vehicles.Commands;

public class DeleteVehicleCommand : ICommand
{
    public Guid Id { get; set; }
}

public class DeleteVehicleCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<DeleteVehicleCommand>
{
    public async ValueTask<Unit> Handle(DeleteVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == command.Id && v.OwnerId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), command.Id);

        vehicle.SoftDelete();
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
