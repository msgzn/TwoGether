using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Common;
using TwoGether.Domain.Matches;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Users;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<PhoneVerification> PhoneVerifications => Set<PhoneVerification>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<DriverRoute> DriverRoutes => Set<DriverRoute>();
    public DbSet<PassengerRoute> PassengerRoutes => Set<PassengerRoute>();
    public DbSet<RouteMatch> RouteMatches => Set<RouteMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplyEntityConventions(modelBuilder);
    }

    private static void ApplyEntityConventions(ModelBuilder modelBuilder)
    {
        var rootEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t.ClrType) && t.BaseType is null)
            .ToList();

        foreach (var entityType in rootEntityTypes)
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            builder.Property(nameof(Entity.Id)).ValueGeneratedNever();

            var entity = Expression.Parameter(entityType.ClrType, "e");
            var notDeleted = Expression.Lambda(
                Expression.Not(Expression.Property(entity, nameof(Entity.IsDeleted))), entity);
            builder.HasQueryFilter(QueryFilters.SoftDelete, notDeleted);

            if (typeof(AggregateRoot).IsAssignableFrom(entityType.ClrType))
                builder.Property<uint>("Version").HasColumnName("xmin").HasColumnType("xid").IsRowVersion();
        }
    }
}
