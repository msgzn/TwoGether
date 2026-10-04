using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwoGether.Application.Auth;
using TwoGether.Application.Matches;

namespace TwoGether.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<AuthTokenIssuer>();
        services.AddScoped<RouteMatcher>();
        services.Configure<MatchingOptions>(configuration.GetSection(MatchingOptions.SectionName));

        return services;
    }
}
