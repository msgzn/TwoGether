using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Exceptions;
using TwoGether.Domain.Common;

namespace TwoGether.Persistence;

internal sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        TouchAggregatesWithChangedChildren();

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new ConcurrencyConflictException(e);
        }
    }
    
    private void TouchAggregatesWithChangedChildren()
    {
        var changedOwnerIds = db.ChangeTracker.Entries()
            .Where(e => e.Metadata.IsOwned() && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => e.Property(e.Metadata.FindOwnership()!.Properties[0].Name).CurrentValue)
            .OfType<Guid>()
            .ToHashSet();

        foreach (var root in db.ChangeTracker.Entries<AggregateRoot>())
        {
            if (root.State == EntityState.Unchanged && changedOwnerIds.Contains(root.Entity.Id))
                root.State = EntityState.Modified;
        }
    }
}
