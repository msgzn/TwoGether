using Microsoft.EntityFrameworkCore;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Users;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<PhoneVerification> PhoneVerifications { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
}
