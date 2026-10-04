using TwoGether.Domain.Common;

namespace TwoGether.Application.Abstractions;

public interface IRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken ct);
    void Add(TAggregate aggregate);
    void Remove(TAggregate aggregate);
}
