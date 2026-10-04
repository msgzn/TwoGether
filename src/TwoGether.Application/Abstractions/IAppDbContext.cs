using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Matches;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Users;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<PhoneVerification> PhoneVerifications { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<DriverRoute> DriverRoutes { get; }
    DbSet<PassengerRoute> PassengerRoutes { get; }
    DbSet<RouteMatch> RouteMatches { get; }

    DatabaseFacade Database { get; }
}
