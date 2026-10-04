namespace TwoGether.Application.Exceptions;

public sealed class ConcurrencyConflictException(Exception inner)
    : Exception("The resource was modified concurrently. Please retry.", inner);
