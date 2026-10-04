using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Routes.Commands;

public class CreateDriverRouteCommand : ICommand<DriverRouteDto>
{
    public Guid VehicleId { get; set; }
    public Location Origin { get; set; } = null!;
    public Location Destination { get; set; } = null!;
    public List<DayOfWeek> Days { get; set; } = [];
    public TimeOnly DepartureTime { get; set; }
    public int? Seats { get; set; }
}

public class CreateDriverRouteCommandValidator : AbstractValidator<CreateDriverRouteCommand>
{
    public CreateDriverRouteCommandValidator()
    {
        RuleFor(c => c.VehicleId).NotEmpty();
        RuleFor(c => c.Origin).NotNull().SetValidator(new LocationValidator());
        RuleFor(c => c.Destination).NotNull().SetValidator(new LocationValidator());
        RuleFor(c => c.Days).NotEmpty();
        RuleFor(c => c.Seats).InclusiveBetween(Vehicle.MinPassengerSeats, Vehicle.MaxPassengerSeats);
    }
}

public class CreateDriverRouteCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IRoutingService routing,
    RouteMatcher matcher) : ICommandHandler<CreateDriverRouteCommand, DriverRouteDto>
{
    public async ValueTask<DriverRouteDto> Handle(CreateDriverRouteCommand command, CancellationToken ct)
    {
        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == command.VehicleId && v.OwnerId == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), command.VehicleId);

        var origin = command.Origin.ToPoint();
        var destination = command.Destination.ToPoint();
        ServiceArea.EnsureContains(origin);
        ServiceArea.EnsureContains(destination);

        var path = await routing.BuildRouteAsync(origin, destination, ct);

        var route = DriverRoute.Create(
            currentUser.UserId,
            vehicle,
            origin,
            destination,
            path,
            RideDaysMapper.ToRideDays(command.Days),
            command.DepartureTime,
            command.Seats);

        db.DriverRoutes.Add(route);
        await unitOfWork.SaveChangesAsync(ct);

        await matcher.RecomputeAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return DriverRouteDto.From(route);
    }
}
