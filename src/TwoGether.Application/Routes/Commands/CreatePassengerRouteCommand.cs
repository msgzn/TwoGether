using FluentValidation;
using Mediator;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Matches;
using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes.Commands;

public class CreatePassengerRouteCommand : ICommand<PassengerRouteDto>
{
    public Location Origin { get; set; } = null!;
    public Location Destination { get; set; } = null!;
    public List<DayOfWeek> Days { get; set; } = [];
    public TimeOnly EarliestPickup { get; set; }
    public TimeOnly LatestPickup { get; set; }
}

public class CreatePassengerRouteCommandValidator : AbstractValidator<CreatePassengerRouteCommand>
{
    public CreatePassengerRouteCommandValidator()
    {
        RuleFor(c => c.Origin).NotNull().SetValidator(new LocationValidator());
        RuleFor(c => c.Destination).NotNull().SetValidator(new LocationValidator());
        RuleFor(c => c.Days).NotEmpty();
    }
}

public class CreatePassengerRouteCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    RouteMatcher matcher) : ICommandHandler<CreatePassengerRouteCommand, PassengerRouteDto>
{
    public async ValueTask<PassengerRouteDto> Handle(CreatePassengerRouteCommand command, CancellationToken ct)
    {
        var route = PassengerRoute.Create(
            currentUser.UserId,
            command.Origin.ToPoint(),
            command.Destination.ToPoint(),
            RideDaysMapper.ToRideDays(command.Days),
            command.EarliestPickup,
            command.LatestPickup);

        db.PassengerRoutes.Add(route);
        await unitOfWork.SaveChangesAsync(ct);

        await matcher.RecomputeAsync(route, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return PassengerRouteDto.From(route);
    }
}
