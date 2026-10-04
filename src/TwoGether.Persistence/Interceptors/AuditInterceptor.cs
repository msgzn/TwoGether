using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TwoGether.Domain.Common;

namespace TwoGether.Persistence.Interceptors;

internal sealed class AuditInterceptor(TimeProvider clock) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Apply(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void Apply(DbContext? db)
    {
        if (db is null)
            return;

        var now = clock.GetUtcNow();

        foreach (var entry in db.ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetValue(entry, nameof(Entity.InsertedAt), now);
                    break;

                case EntityState.Modified:
                    SetValue(entry, nameof(Entity.UpdatedAt), now);
                    break;
            }
        }
    }

    private static void SetValue(EntityEntry entry, string property, object value) =>
        entry.Property(property).CurrentValue = value;
}
