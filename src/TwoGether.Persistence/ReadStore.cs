using Microsoft.EntityFrameworkCore;
using TwoGether.Application.Abstractions;

namespace TwoGether.Persistence;

public class ReadStore(AppDbContext db) : IReadStore
{
    public IQueryable<T> Query<T>() where T : class => db.Set<T>().AsNoTracking();
}