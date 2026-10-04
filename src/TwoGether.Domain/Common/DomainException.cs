namespace TwoGether.Domain.Common;

/// <summary>
/// A business rule was violated. <see cref="Code"/> is stable and safe to expose to clients.
/// </summary>
public sealed class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}
