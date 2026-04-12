using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Internal;
using HTT.AspNetCore.OpenApi.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HTT.AspNetCore.OpenApi.Extensions
{
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

            if (options.UiProvider == CApiDocsUiProvider.None)
                return app;

            // ── Mount UI providers ───────────────────────────────────────
            var allProviders = app.Services
                .GetServices<IApiDocsUiProvider>()
                .ToList();

            var activeProviders = options.UiProvider == CApiDocsUiProvider.All
                ? allProviders
                : allProviders.Where(p => p.ProviderType == options.UiProvider);

            foreach (var provider in activeProviders)
            {
                provider.Map(app, options, documentNames);
            }

            return app;
        }
    }
}