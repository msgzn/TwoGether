using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwoGether.Application.Abstractions;
using TwoGether.Infrastructure.Auth;
using TwoGether.Infrastructure.Sms;

namespace TwoGether.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => Encoding.UTF8.GetByteCount(o.SigningKey) >= 32, "Jwt:SigningKey must be at least 32 bytes.")
            .Validate(o => o.Issuer != "" && o.Audience != "", "Jwt:Issuer and Jwt:Audience are required.")
            .ValidateOnStart();

        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<ISmsSender, LoggingSmsSender>();

        return services;
    }
}
