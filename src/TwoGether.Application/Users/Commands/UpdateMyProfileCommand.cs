using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Domain.Users;

namespace TwoGether.Application.Users.Commands;

public class UpdateMyProfileCommand : ICommand<UserDto>
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(User.NameMaxLength);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(User.NameMaxLength);
    }
}

public class UpdateMyProfileCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<UpdateMyProfileCommand, UserDto>
{
    public async ValueTask<UserDto> Handle(UpdateMyProfileCommand command, CancellationToken ct)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(User), currentUser.UserId);

        user.UpdateProfile(command.FirstName, command.LastName);
        await unitOfWork.SaveChangesAsync(ct);

        return new UserDto(user.Id, user.PhoneNumber.Value, user.FirstName, user.LastName, user.IsProfileCompleted);
    }
}
