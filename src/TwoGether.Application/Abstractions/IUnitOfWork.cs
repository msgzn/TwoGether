namespace TwoGether.Application.Abstractions;

public interface IUnitOfWork
{
    /// <summary>Atomically persists every change made to tracked aggregates.</summary>
    Task SaveChangesAsync(CancellationToken ct);
}
