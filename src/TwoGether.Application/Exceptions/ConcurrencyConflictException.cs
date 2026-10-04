namespace TwoGether.Application.Exceptions;

/// <summary>The aggregate was changed by someone else since it was loaded. Safe for the client to retry.</summary>
public sealed class ConcurrencyConflictException(Exception inner)
    : Exception("The resource was modified concurrently. Please retry.", inner);
