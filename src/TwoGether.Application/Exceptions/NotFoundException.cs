namespace TwoGether.Application.Exceptions;

public sealed class NotFoundException(string resource, object id)
    : Exception($"{resource} '{id}' was not found.");
