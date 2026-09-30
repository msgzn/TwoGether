using TwoGether.Application.Abstractions;

namespace TwoGether.Persistence;

public class Repository<T>(AppDbContext db) : IRepository<T> where T : class
{
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.Set<T>().FindAsync([id], ct);

    public async Task AddAsync(T entity, CancellationToken ct)
        => await db.Set<T>().AddAsync(entity, ct);

    public void Remove(T entity) => db.Set<T>().Remove(entity);
}