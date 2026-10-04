using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;

namespace TwoGether.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteFilter(modelBuilder);
    }
    
    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        var rootEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t.ClrType) && t.BaseType is null);

        foreach (var entityType in rootEntityTypes)
        {
            var entity = Expression.Parameter(entityType.ClrType, "e");
            var notDeleted = Expression.Lambda(
                Expression.Not(Expression.Property(entity, nameof(Entity.IsDeleted))), entity);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(QueryFilters.SoftDelete, notDeleted);
        }
    }
}
