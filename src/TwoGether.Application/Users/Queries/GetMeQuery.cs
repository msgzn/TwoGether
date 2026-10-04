using Mediator;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Domain.Users;

namespace TwoGether.Application.Users.Queries;

public class GetMeQuery : IQuery<UserDto>;

public class GetMeQueryHandler(IAppDbContext db, ICurrentUser currentUser) : IQueryHandler<GetMeQuery, UserDto>
{
    public async ValueTask<UserDto> Handle(GetMeQuery query, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(User), currentUser.UserId);

        return new UserDto(user.Id, user.PhoneNumber.Value, user.FirstName, user.LastName, user.IsProfileCompleted);
    }
}
