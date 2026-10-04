using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TwoGether.Application;

/// <summary>
/// Handlers and pipeline behaviors are registered by the Mediator source generator in the Api project (AddMediator).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
