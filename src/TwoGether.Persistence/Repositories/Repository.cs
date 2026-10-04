using TwoGether.Application.Abstractions;
using TwoGether.Domain.Common;

namespace TwoGether.Persistence.Repositories;

internal class Repository<TAggregate>(AppDbContext db) : IRepository<TAggregate>
    where TAggregate : AggregateRoot
{
    protected AppDbContext Db { get; } = db;

    public async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await Db.Set<TAggregate>().FindAsync([id], ct);

    public void Add(TAggregate aggregate) => Db.Set<TAggregate>().Add(aggregate);

    public void Remove(TAggregate aggregate) => Db.Set<TAggregate>().Remove(aggregate);
}
