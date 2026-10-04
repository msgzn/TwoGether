using Mediator;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;
using TwoGether.Api.Infrastructure;
using TwoGether.Application;
using TwoGether.Application.Abstractions;
using TwoGether.Application.Behaviors;
using TwoGether.Infrastructure;
using TwoGether.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, logger) => logger
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddPersistence(builder.Configuration);

    builder.Services.AddMediator((MediatorOptions options) =>
    {
        options.ServiceLifetime = ServiceLifetime.Scoped;
        options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
    });

    builder.Services.AddControllers();
    builder.Services.AddApiAuthentication();
    builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ApiExceptionHandler>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, ClaimsCurrentUser>();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
        options.EnrichDiagnosticContext = (diagnostics, http) =>
            diagnostics.Set("UserId", http.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value));

    app.UseExceptionHandler();
    app.UseStatusCodePages();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "TwoGether API"));
        await app.Services.MigrateDatabaseAsync();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public abstract partial class Program;