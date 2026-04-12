# HTT.AspNetCore — OpenApi Module (Full Source)

---

## HTT.AspNetCore.csproj (relevant section)

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.OpenApi"        Version="9.0.4" />
  <PackageReference Include="Scalar.AspNetCore"                   Version="2.3.5" />
  <PackageReference Include="Swashbuckle.AspNetCore.SwaggerUI"    Version="7.3.0" />
  <PackageReference Include="Swashbuckle.AspNetCore.ReDoc"        Version="7.3.0" />
  <PackageReference Include="Microsoft.OpenApi"                   Version="1.6.22" />
</ItemGroup>
```

---

## Options/ApiDocsUiProvider.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Options;

/// <summary>Available UI providers for rendering interactive API documentation.</summary>
public enum ApiDocsUiProvider
{
    /// <summary>No UI — only the raw OpenAPI JSON endpoint is served.</summary>
    None = 0,

    /// <summary>
    /// Scalar — modern, fast, visually rich API reference.
    /// Route: <c>/{ScalarRoutePrefix}/{documentName}</c>.
    /// </summary>
    Scalar = 1,

    /// <summary>
    /// Swagger UI (Swashbuckle) — classic, widely adopted interactive docs.
    /// Route: <c>/{SwaggerUiRoutePrefix}</c>.
    /// </summary>
    SwaggerUi = 2,

    /// <summary>
    /// ReDoc — clean, responsive three-panel documentation.
    /// Route: <c>/{ReDocRoutePrefix}</c>.
    /// </summary>
    ReDoc = 3,

    /// <summary>All providers simultaneously (Scalar + Swagger UI + ReDoc).</summary>
    All = 99
}
```

---

## Options/ApiVersionDocument.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Options;

/// <summary>Defines a named API version document.</summary>
public sealed class ApiVersionDocument
{
    /// <summary>Document name used in the route (e.g. <c>"v1"</c>, <c>"v2"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>Display title for this version.</summary>
    public string? Title { get; set; }

    /// <summary>Description for this version.</summary>
    public string? Description { get; set; }
}
```

---

## Options/ApiSecurityScheme.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Options;

/// <summary>Defines a security scheme for the OpenAPI document.</summary>
public sealed class ApiSecurityScheme
{
    /// <summary>Scheme identifier (e.g. <c>"Bearer"</c>, <c>"ApiKey"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>
    /// Scheme type: <c>"Http"</c>, <c>"ApiKey"</c>, <c>"OAuth2"</c>, <c>"OpenIdConnect"</c>.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>Where the credential is sent: <c>"Header"</c>, <c>"Query"</c>, <c>"Cookie"</c>.</summary>
    public string Location { get; set; } = "Header";

    /// <summary>Header / query parameter name (for ApiKey schemes).</summary>
    public string? ParameterName { get; set; }

    /// <summary>HTTP scheme (e.g. <c>"bearer"</c> for JWT).</summary>
    public string? Scheme { get; set; }

    /// <summary>Bearer format hint (e.g. <c>"JWT"</c>).</summary>
    public string? BearerFormat { get; set; }

    /// <summary>Description shown in the docs.</summary>
    public string? Description { get; set; }

    /// <summary>OpenID Connect discovery URL.</summary>
    public string? OpenIdConnectUrl { get; set; }

    /// <summary>OAuth2 authorization URL.</summary>
    public string? AuthorizationUrl { get; set; }

    /// <summary>OAuth2 token URL.</summary>
    public string? TokenUrl { get; set; }

    /// <summary>OAuth2 refresh URL.</summary>
    public string? RefreshUrl { get; set; }

    /// <summary>OAuth2 scopes as <c>"scope": "description"</c>.</summary>
    public Dictionary<string, string>? Scopes { get; set; }
}
```

---

## Options/ApiServerEntry.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Options;

/// <summary>Defines a server entry in the OpenAPI document.</summary>
public sealed class ApiServerEntry
{
    /// <summary>Server URL (e.g. <c>"https://api.example.com"</c>).</summary>
    public required string Url { get; set; }

    /// <summary>Description of the server (e.g. <c>"Production"</c>).</summary>
    public string? Description { get; set; }
}
```

---

## Options/HttOpenApiOptions.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Options;

/// <summary>
/// Centralized configuration options for the HTT OpenAPI documentation module.
/// Bind from <c>appsettings.json</c> section <c>"OpenApi"</c>.
/// </summary>
public sealed class HttOpenApiOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "OpenApi";

    // ─── General ─────────────────────────────────────────────────────

    /// <summary>Whether the OpenAPI documentation is enabled (default: <c>true</c>).</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Environments where the API docs UI is available.
    /// If empty, available in all environments.
    /// Example: <c>["Development", "Staging"]</c>.
    /// </summary>
    public string[] AllowedEnvironments { get; set; } = ["Development", "Staging"];

    /// <summary>Which UI provider to use (default: <see cref="ApiDocsUiProvider.Scalar"/>).</summary>
    public ApiDocsUiProvider UiProvider { get; set; } = ApiDocsUiProvider.Scalar;

    // ─── Document metadata ───────────────────────────────────────────

    /// <summary>Service / API title displayed in the docs.</summary>
    public string Title { get; set; } = "HTT API";

    /// <summary>Short description of the service.</summary>
    public string? Description { get; set; }

    /// <summary>API version string (e.g. <c>"v1"</c>, <c>"v2"</c>).</summary>
    public string Version { get; set; } = "v1";

    /// <summary>Contact name shown in the OpenAPI info object.</summary>
    public string? ContactName { get; set; }

    /// <summary>Contact email shown in the OpenAPI info object.</summary>
    public string? ContactEmail { get; set; }

    /// <summary>Contact URL shown in the OpenAPI info object.</summary>
    public string? ContactUrl { get; set; }

    /// <summary>License name (e.g. <c>"MIT"</c>).</summary>
    public string? LicenseName { get; set; }

    /// <summary>License URL.</summary>
    public string? LicenseUrl { get; set; }

    /// <summary>Terms of Service URL.</summary>
    public string? TermsOfServiceUrl { get; set; }

    /// <summary>External documentation URL.</summary>
    public string? ExternalDocsUrl { get; set; }

    /// <summary>External documentation description.</summary>
    public string? ExternalDocsDescription { get; set; }

    // ─── Build metadata ──────────────────────────────────────────────

    /// <summary>
    /// Additional key-value metadata injected into the OpenAPI info extensions.
    /// Useful for build hash, deployment timestamp, environment name, etc.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];

    // ─── API versioning ──────────────────────────────────────────────

    /// <summary>
    /// Named OpenAPI documents. Each entry produces a separate <c>/openapi/{name}.json</c>.
    /// When empty, a single document named <see cref="Version"/> is generated.
    /// </summary>
    public List<ApiVersionDocument> Documents { get; set; } = [];

    // ─── Security ────────────────────────────────────────────────────

    /// <summary>Security schemes to register in the OpenAPI document.</summary>
    public List<ApiSecurityScheme> SecuritySchemes { get; set; } = [];

    // ─── Route customization ─────────────────────────────────────────

    /// <summary>Route prefix for the raw OpenAPI JSON endpoint (default: <c>"openapi"</c>).</summary>
    public string OpenApiRoutePrefix { get; set; } = "openapi";

    /// <summary>Route prefix for Scalar UI (default: <c>"scalar"</c>).</summary>
    public string ScalarRoutePrefix { get; set; } = "scalar";

    /// <summary>Route prefix for Swagger UI (default: <c>"swagger"</c>).</summary>
    public string SwaggerUiRoutePrefix { get; set; } = "swagger";

    /// <summary>Route prefix for ReDoc UI (default: <c>"redoc"</c>).</summary>
    public string ReDocRoutePrefix { get; set; } = "redoc";

    // ─── Server URLs ─────────────────────────────────────────────────

    /// <summary>
    /// Server URLs to include in the OpenAPI document.
    /// Useful for gateway / public-facing URLs.
    /// </summary>
    public List<ApiServerEntry> Servers { get; set; } = [];
}
```

---

## Internal/OpenApiRouteHelper.cs

```csharp
namespace HTT.AspNetCore.OpenApi.Internal;

/// <summary>Builds consistent route patterns for OpenAPI JSON and UI endpoints.</summary>
internal static class OpenApiRouteHelper
{
    /// <summary>
    /// Returns the absolute path for an OpenAPI JSON document.
    /// Example: <c>/openapi/v1.json</c>.
    /// </summary>
    public static string JsonEndpoint(string prefix, string documentName)
        => $"/{prefix}/{documentName}.json";

    /// <summary>
    /// Returns the absolute path pattern for OpenAPI JSON using a placeholder.
    /// Example: <c>/openapi/{documentName}.json</c>.
    /// </summary>
    public static string JsonEndpointPattern(string prefix)
        => $"/{prefix}/{{documentName}}.json";

    /// <summary>
    /// Returns the absolute path for a UI endpoint root.
    /// Example: <c>/scalar</c>.
    /// </summary>
    public static string UiEndpoint(string prefix)
        => $"/{prefix}";
}
```

---

## Internal/SecuritySchemeMapper.cs

```csharp
using HTT.AspNetCore.OpenApi.Options;
using Microsoft.OpenApi.Models;

namespace HTT.AspNetCore.OpenApi.Internal;

/// <summary>Maps <see cref="ApiSecurityScheme"/> config objects to <see cref="OpenApiSecurityScheme"/>.</summary>
internal static class SecuritySchemeMapper
{
    public static OpenApiSecurityScheme Map(ApiSecurityScheme source)
    {
        var type = source.Type.ToUpperInvariant() switch
        {
            "HTTP"           => SecuritySchemeType.Http,
            "APIKEY"         => SecuritySchemeType.ApiKey,
            "OAUTH2"         => SecuritySchemeType.OAuth2,
            "OPENIDCONNECT"  => SecuritySchemeType.OpenIdConnect,
            _                => SecuritySchemeType.Http
        };

        var location = source.Location.ToUpperInvariant() switch
        {
            "QUERY"  => ParameterLocation.Query,
            "COOKIE" => ParameterLocation.Cookie,
            _        => ParameterLocation.Header
        };

        var scheme = new OpenApiSecurityScheme
        {
            Name         = source.ParameterName ?? source.Name,
            Type         = type,
            In           = location,
            Scheme       = source.Scheme,
            BearerFormat = source.BearerFormat,
            Description  = source.Description
        };

        if (type == SecuritySchemeType.OpenIdConnect && source.OpenIdConnectUrl is not null)
        {
            scheme.OpenIdConnectUrl = new Uri(source.OpenIdConnectUrl);
        }

        if (type == SecuritySchemeType.OAuth2)
        {
            scheme.Flows = BuildOAuthFlows(source);
        }

        return scheme;
    }

    // ─── Private ──────────────────────────────────────────────────────

    private static OpenApiOAuthFlows BuildOAuthFlows(ApiSecurityScheme source)
    {
        var scopes = source.Scopes ?? [];
        var hasAuthUrl  = source.AuthorizationUrl is not null;
        var hasTokenUrl = source.TokenUrl is not null;
        var refreshUrl  = source.RefreshUrl is not null ? new Uri(source.RefreshUrl) : null;

        return new OpenApiOAuthFlows
        {
            // Both URLs → Authorization Code flow
            AuthorizationCode = hasAuthUrl && hasTokenUrl
                ? new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(source.AuthorizationUrl!),
                    TokenUrl         = new Uri(source.TokenUrl!),
                    RefreshUrl       = refreshUrl,
                    Scopes           = scopes
                }
                : null,

            // Token URL only → Client Credentials flow
            ClientCredentials = hasTokenUrl && !hasAuthUrl
                ? new OpenApiOAuthFlow
                {
                    TokenUrl   = new Uri(source.TokenUrl!),
                    RefreshUrl = refreshUrl,
                    Scopes     = scopes
                }
                : null,

            // Auth URL only → Implicit flow
            Implicit = hasAuthUrl && !hasTokenUrl
                ? new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(source.AuthorizationUrl!),
                    RefreshUrl       = refreshUrl,
                    Scopes           = scopes
                }
                : null
        };
    }
}
```

---

## Transformers/Documents/MetadataDocumentTransformer.cs

```csharp
using HTT.AspNetCore.OpenApi.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Populates the OpenAPI <c>info</c> object from <see cref="HttOpenApiOptions"/>:
/// title, description, version, contact, license, terms of service,
/// external docs, and custom metadata extensions.
/// </summary>
internal sealed class MetadataDocumentTransformer(IOptions<HttOpenApiOptions> options)
    : IOpenApiDocumentTransformer
{
    private readonly HttOpenApiOptions _options = options.Value;

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Per-document override when versioning is used
        var versionDoc = _options.Documents
            .FirstOrDefault(d => d.Name == context.DocumentName);

        document.Info ??= new OpenApiInfo();
        document.Info.Title       = versionDoc?.Title       ?? _options.Title;
        document.Info.Description = versionDoc?.Description ?? _options.Description;
        document.Info.Version     = context.DocumentName;

        if (_options.TermsOfServiceUrl is not null)
            document.Info.TermsOfService = new Uri(_options.TermsOfServiceUrl);

        if (_options.ContactName is not null
            || _options.ContactEmail is not null
            || _options.ContactUrl is not null)
        {
            document.Info.Contact = new OpenApiContact
            {
                Name  = _options.ContactName,
                Email = _options.ContactEmail,
                Url   = _options.ContactUrl is not null ? new Uri(_options.ContactUrl) : null
            };
        }

        if (_options.LicenseName is not null)
        {
            document.Info.License = new OpenApiLicense
            {
                Name = _options.LicenseName,
                Url  = _options.LicenseUrl is not null ? new Uri(_options.LicenseUrl) : null
            };
        }

        if (_options.ExternalDocsUrl is not null)
        {
            document.ExternalDocs = new OpenApiExternalDocs
            {
                Url         = new Uri(_options.ExternalDocsUrl),
                Description = _options.ExternalDocsDescription
            };
        }

        // Inject custom key-value metadata as OpenAPI extensions (x-*)
        foreach (var (key, value) in _options.Metadata)
        {
            document.Info.Extensions[$"x-{key}"] = new OpenApiString(value);
        }

        return Task.CompletedTask;
    }
}
```

---

## Transformers/Documents/ServerUrlDocumentTransformer.cs

```csharp
using HTT.AspNetCore.OpenApi.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Replaces the auto-detected server list with the explicit <see cref="HttOpenApiOptions.Servers"/>
/// entries. Useful for gateways, reverse proxies, or multi-environment deployments.
/// No-op when <see cref="HttOpenApiOptions.Servers"/> is empty.
/// </summary>
internal sealed class ServerUrlDocumentTransformer(IOptions<HttOpenApiOptions> options)
    : IOpenApiDocumentTransformer
{
    private readonly HttOpenApiOptions _options = options.Value;

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (_options.Servers.Count == 0)
            return Task.CompletedTask;

        document.Servers = _options.Servers
            .Select(s => new OpenApiServer
            {
                Url         = s.Url,
                Description = s.Description
            })
            .ToList();

        return Task.CompletedTask;
    }
}
```

---

## Transformers/Documents/SecuritySchemeDocumentTransformer.cs

```csharp
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Registers all <see cref="HttOpenApiOptions.SecuritySchemes"/> into
/// <c>components/securitySchemes</c> of the OpenAPI document.
/// No-op when the list is empty.
/// </summary>
internal sealed class SecuritySchemeDocumentTransformer(IOptions<HttOpenApiOptions> options)
    : IOpenApiDocumentTransformer
{
    private readonly HttOpenApiOptions _options = options.Value;

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (_options.SecuritySchemes.Count == 0)
            return Task.CompletedTask;

        document.Components ??= new OpenApiComponents();

        foreach (var scheme in _options.SecuritySchemes)
        {
            document.Components.SecuritySchemes[scheme.Name] =
                SecuritySchemeMapper.Map(scheme);
        }

        return Task.CompletedTask;
    }
}
```

---

## Transformers/Operations/DefaultResponseOperationTransformer.cs

```csharp
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace HTT.AspNetCore.OpenApi.Transformers.Operations;

/// <summary>
/// Injects standard HTTP error responses (400, 401, 403, 500) with
/// <c>application/problem+json</c> content into every operation that
/// does not already define them.
/// </summary>
internal sealed class DefaultResponseOperationTransformer : IOpenApiOperationTransformer
{
    private static readonly IReadOnlyDictionary<string, OpenApiResponse> DefaultErrorResponses =
        new Dictionary<string, OpenApiResponse>
        {
            ["400"] = Problem("Bad Request",            "Invalid input or validation failure."),
            ["401"] = Problem("Unauthorized",           "Missing or invalid authentication credentials."),
            ["403"] = Problem("Forbidden",              "Insufficient permissions to perform this action."),
            ["500"] = Problem("Internal Server Error",  "An unexpected error occurred on the server.")
        };

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        foreach (var (statusCode, response) in DefaultErrorResponses)
        {
            // Respect any response the developer has explicitly declared
            operation.Responses.TryAdd(statusCode, response);
        }

        return Task.CompletedTask;
    }

    // ─── Private ──────────────────────────────────────────────────────

    private static OpenApiResponse Problem(string description, string detail) => new()
    {
        Description = $"{description} — {detail}",
        Content = new Dictionary<string, OpenApiMediaType>
        {
            ["application/problem+json"] = new()
            {
                Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id   = "ProblemDetails"
                    }
                }
            }
        }
    };
}
```

---

## Providers/IApiDocsUiProvider.cs

```csharp
using HTT.AspNetCore.OpenApi.Options;

namespace HTT.AspNetCore.OpenApi.Providers;

/// <summary>
/// Contract for registering an API documentation UI into the request pipeline.
/// Each implementation handles exactly one <see cref="ApiDocsUiProvider"/> value.
/// </summary>
public interface IApiDocsUiProvider
{
    /// <summary>Identifies which <see cref="ApiDocsUiProvider"/> this implementation handles.</summary>
    ApiDocsUiProvider ProviderType { get; }

    /// <summary>Registers routes and/or middleware for this UI onto the application.</summary>
    /// <param name="app">The running web application.</param>
    /// <param name="options">Resolved OpenAPI options.</param>
    /// <param name="documentNames">Ordered list of all active document names.</param>
    void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames);
}
```

---

## Providers/ScalarDocsUiProvider.cs

```csharp
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Options;
using Scalar.AspNetCore;

namespace HTT.AspNetCore.OpenApi.Providers;

/// <summary>Registers the Scalar interactive API reference UI.</summary>
internal sealed class ScalarDocsUiProvider : IApiDocsUiProvider
{
    public ApiDocsUiProvider ProviderType => ApiDocsUiProvider.Scalar;

    public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
    {
        app.MapScalarApiReference(scalar =>
        {
            scalar.Title                = options.Title;
            scalar.EndpointPathPrefix   = $"{OpenApiRouteHelper.UiEndpoint(options.ScalarRoutePrefix)}/{{documentName}}";
            scalar.OpenApiRoutePattern  = OpenApiRouteHelper.JsonEndpointPattern(options.OpenApiRoutePrefix);

            scalar.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }
}
```

---

## Providers/SwaggerDocsUiProvider.cs

```csharp
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Options;

namespace HTT.AspNetCore.OpenApi.Providers;

/// <summary>Registers the Swagger UI (Swashbuckle) interactive docs.</summary>
internal sealed class SwaggerDocsUiProvider : IApiDocsUiProvider
{
    public ApiDocsUiProvider ProviderType => ApiDocsUiProvider.SwaggerUi;

    public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
    {
        app.UseSwaggerUI(ui =>
        {
            ui.RoutePrefix = options.SwaggerUiRoutePrefix;

            foreach (var name in documentNames)
            {
                ui.SwaggerEndpoint(
                    url:  OpenApiRouteHelper.JsonEndpoint(options.OpenApiRoutePrefix, name),
                    name: name);
            }

            ui.DocumentTitle = options.Title;
        });
    }
}
```

---

## Providers/ReDocDocsUiProvider.cs

```csharp
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Options;

namespace HTT.AspNetCore.OpenApi.Providers;

/// <summary>
/// Registers the ReDoc three-panel documentation UI.
/// When multiple documents are configured, each gets its own route:
/// <c>/redoc/v1</c>, <c>/redoc/v2</c>, etc.
/// </summary>
internal sealed class ReDocDocsUiProvider : IApiDocsUiProvider
{
    public ApiDocsUiProvider ProviderType => ApiDocsUiProvider.ReDoc;

    public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
    {
        var multipleDocuments = documentNames.Count > 1;

        foreach (var name in documentNames)
        {
            // Single doc → /redoc   |   Multiple docs → /redoc/v1, /redoc/v2, …
            var routePrefix = multipleDocuments
                ? $"{options.ReDocRoutePrefix}/{name}"
                : options.ReDocRoutePrefix;

            app.UseReDoc(redoc =>
            {
                redoc.RoutePrefix  = routePrefix;
                redoc.DocumentPath = OpenApiRouteHelper.JsonEndpoint(options.OpenApiRoutePrefix, name);
                redoc.DocumentTitle = multipleDocuments
                    ? $"{options.Title} — {name}"
                    : options.Title;
            });
        }
    }
}
```

---

## Extensions/OpenApiServiceCollectionExtensions.cs

```csharp
using HTT.AspNetCore.OpenApi.Options;
using HTT.AspNetCore.OpenApi.Providers;
using HTT.AspNetCore.OpenApi.Transformers.Documents;
using HTT.AspNetCore.OpenApi.Transformers.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HTT.AspNetCore.OpenApi.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods for the HTT OpenAPI module.
/// </summary>
public static class OpenApiServiceCollectionExtensions
{
    /// <summary>
    /// Registers HTT OpenAPI services: document generation, transformers, and UI providers.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configuration">App configuration (binds <c>"OpenApi"</c> section).</param>
    /// <param name="configure">Optional inline override of options after config binding.</param>
    public static IServiceCollection AddHttOpenApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttOpenApiOptions>? configure = null)
    {
        // Bind and register options
        services.Configure<HttOpenApiOptions>(
            configuration.GetSection(HttOpenApiOptions.SectionName));

        if (configure is not null)
            services.PostConfigure<HttOpenApiOptions>(configure);

        // Materialize options to drive registration decisions
        var options = configuration
            .GetSection(HttOpenApiOptions.SectionName)
            .Get<HttOpenApiOptions>() ?? new HttOpenApiOptions();

        configure?.Invoke(options);

        if (!options.Enabled)
            return services;

        // ── UI Providers ────────────────────────────────────────────
        services.AddTransient<IApiDocsUiProvider, ScalarDocsUiProvider>();
        services.AddTransient<IApiDocsUiProvider, SwaggerDocsUiProvider>();
        services.AddTransient<IApiDocsUiProvider, ReDocDocsUiProvider>();

        // ── OpenAPI document generation ──────────────────────────────
        var documentNames = options.Documents.Count > 0
            ? options.Documents.Select(d => d.Name).ToList()
            : [options.Version];

        foreach (var docName in documentNames)
        {
            services.AddOpenApi(docName, apiOptions =>
            {
                apiOptions.AddDocumentTransformer<MetadataDocumentTransformer>();
                apiOptions.AddDocumentTransformer<ServerUrlDocumentTransformer>();
                apiOptions.AddDocumentTransformer<SecuritySchemeDocumentTransformer>();
                apiOptions.AddOperationTransformer<DefaultResponseOperationTransformer>();
            });
        }

        return services;
    }
}
```

---

## Extensions/OpenApWebApplicationExtensions.cs

```csharp
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Options;
using HTT.AspNetCore.OpenApi.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HTT.AspNetCore.OpenApi.Extensions;

/// <summary>
/// <see cref="WebApplication"/> extension methods for the HTT OpenAPI module.
/// </summary>
public static class OpenApWebApplicationExtensions
{
    /// <summary>
    /// Maps OpenAPI JSON endpoints and mounts the configured UI provider(s)
    /// into the request pipeline.
    /// </summary>
    public static WebApplication UseHttOpenApi(this WebApplication app)
    {
        var options = app.Services
            .GetRequiredService<IOptions<HttOpenApiOptions>>()
            .Value;

        if (!options.Enabled)
            return app;

        // ── Environment guard ────────────────────────────────────────
        if (options.AllowedEnvironments.Length > 0
            && !options.AllowedEnvironments.Contains(
                app.Environment.EnvironmentName,
                StringComparer.OrdinalIgnoreCase))
        {
            return app;
        }

        // ── OpenAPI JSON endpoints ───────────────────────────────────
        var documentNames = options.Documents.Count > 0
            ? options.Documents.Select(d => d.Name).ToList()
            : [options.Version];

        foreach (var name in documentNames)
        {
            app.MapOpenApi(
                OpenApiRouteHelper.JsonEndpoint(options.OpenApiRoutePrefix, name));
        }

        if (options.UiProvider == ApiDocsUiProvider.None)
            return app;

        // ── Mount UI providers ───────────────────────────────────────
        var allProviders = app.Services
            .GetServices<IApiDocsUiProvider>()
            .ToList();

        var activeProviders = options.UiProvider == ApiDocsUiProvider.All
            ? allProviders
            : allProviders.Where(p => p.ProviderType == options.UiProvider);

        foreach (var provider in activeProviders)
        {
            provider.Map(app, options, documentNames);
        }

        return app;
    }
}
```

---

## Usage — Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttOpenApi(builder.Configuration);
// or with inline override:
// builder.Services.AddHttOpenApi(builder.Configuration, opt => opt.UiProvider = ApiDocsUiProvider.All);

var app = builder.Build();

app.UseHttOpenApi();

app.Run();
```

---

## Usage — appsettings.json

```json
{
  "OpenApi": {
    "Enabled": true,
    "AllowedEnvironments": ["Development", "Staging"],
    "UiProvider": "All",
    "Title": "HTT API",
    "Description": "Internal platform services.",
    "Version": "v1",
    "ContactName": "HTT Team",
    "ContactEmail": "api@htt.dev",
    "LicenseName": "MIT",
    "TermsOfServiceUrl": "https://htt.dev/tos",
    "ExternalDocsUrl": "https://docs.htt.dev",
    "ExternalDocsDescription": "Full developer documentation",
    "Metadata": {
      "build-hash": "abc123",
      "deployed-at": "2025-04-10T00:00:00Z"
    },
    "Documents": [
      { "Name": "v1", "Title": "HTT API v1", "Description": "Stable release." },
      { "Name": "v2", "Title": "HTT API v2", "Description": "Preview release." }
    ],
    "SecuritySchemes": [
      {
        "Name": "Bearer",
        "Type": "Http",
        "Scheme": "bearer",
        "BearerFormat": "JWT",
        "Description": "Enter your JWT access token."
      }
    ],
    "Servers": [
      { "Url": "https://api.htt.dev", "Description": "Production" },
      { "Url": "https://staging-api.htt.dev", "Description": "Staging" }
    ],
    "OpenApiRoutePrefix": "openapi",
    "ScalarRoutePrefix": "scalar",
    "SwaggerUiRoutePrefix": "swagger",
    "ReDocRoutePrefix": "redoc"
  }
}
```
