namespace TwoGether.Domain.Common;

/// <summary>
/// Consistency boundary: only aggregate roots are loaded and saved through repositories.
/// </summary>
public abstract class AggregateRoot : Entity;
