using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Users;

namespace TwoGether.Application.Auth.Commands;

public class RequestCodeCommand : ICommand<RequestCodeResponse>
{
    public string PhoneNumber { get; set; } = "";
}

public sealed record RequestCodeResponse(DateTimeOffset ExpiresAt, DateTimeOffset ResendAvailableAt);

public class RequestCodeCommandValidator : AbstractValidator<RequestCodeCommand>
{
    public RequestCodeCommandValidator()
    {
        RuleFor(c => c.PhoneNumber).NotEmpty();
    }
}

public class RequestCodeCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    ISmsSender sms,
    TimeProvider clock) : ICommandHandler<RequestCodeCommand, RequestCodeResponse>
{
    public async ValueTask<RequestCodeResponse> Handle(RequestCodeCommand command, CancellationToken ct)
    {
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber);
        var now = clock.GetUtcNow();

        var previous = await db.PhoneVerifications
            .Where(v => v.PhoneNumber == phoneNumber)
            .OrderByDescending(v => v.SentAt)
            .FirstOrDefaultAsync(ct);

        var code = Secrets.GenerateCode();
        var verification = PhoneVerification.Start(phoneNumber, Secrets.Hash(code), now, previous);

        db.PhoneVerifications.Add(verification);
        await unitOfWork.SaveChangesAsync(ct);

        await sms.SendAsync(phoneNumber, $"2gether: ваш код {code}", ct);

        return new RequestCodeResponse(verification.ExpiresAt, now + PhoneVerification.ResendCooldown);
    }
}
