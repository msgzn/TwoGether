using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;
using TwoGether.Domain.Users;

namespace TwoGether.Application.Auth.Commands;

public class VerifyCodeCommand : ICommand<AuthResponse>
{
    public string PhoneNumber { get; set; } = "";
    public string Code { get; set; } = "";
}

public class VerifyCodeCommandValidator : AbstractValidator<VerifyCodeCommand>
{
    public VerifyCodeCommandValidator()
    {
        RuleFor(c => c.PhoneNumber).NotEmpty();
        RuleFor(c => c.Code).NotEmpty().Length(Secrets.CodeLength).Matches("^[0-9]+$");
    }
}

public class VerifyCodeCommandHandler(
    IAppDbContext db,
    IUnitOfWork unitOfWork,
    AuthTokenIssuer tokenIssuer,
    TimeProvider clock) : ICommandHandler<VerifyCodeCommand, AuthResponse>
{
    public async ValueTask<AuthResponse> Handle(VerifyCodeCommand command, CancellationToken ct)
    {
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber);
        var now = clock.GetUtcNow();

        var verification = await db.PhoneVerifications
            .Where(v => v.PhoneNumber == phoneNumber)
            .OrderByDescending(v => v.SentAt)
            .FirstOrDefaultAsync(ct)
            ?? throw new DomainException("auth.code_not_requested", "Request a code first.");

        if (!verification.TryConfirm(Secrets.Hash(command.Code), now))
        {
            await unitOfWork.SaveChangesAsync(ct);
            throw new DomainException("auth.invalid_code", "Invalid code.");
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);
        if (user is null)
        {
            user = User.Register(phoneNumber);
            db.Users.Add(user);
        }

        var response = tokenIssuer.Issue(user, now);
        await unitOfWork.SaveChangesAsync(ct);

        return response;
    }
}
