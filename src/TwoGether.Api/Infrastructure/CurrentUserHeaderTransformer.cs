using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TwoGether.Api.Infrastructure;

/// <summary>
/// Adds the X-User-Id header to the OpenAPI document so Swagger UI shows an "Authorize" button for it.
/// Goes away together with <see cref="HeaderCurrentUser"/> once real auth is in place.
/// </summary>
internal sealed class CurrentUserHeaderTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken ct)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[HeaderCurrentUser.HeaderName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = HeaderCurrentUser.HeaderName,
            Description = "Temporary stand-in for authentication: any user GUID, e.g. 0199a000-0000-7000-8000-000000000001",
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(HeaderCurrentUser.HeaderName, document)] = [],
        });

        return Task.CompletedTask;
    }
}
