using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Providers;
using HTT.AspNetCore.OpenApi.Transformers.Documents;
using HTT.AspNetCore.OpenApi.Transformers.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HTT.AspNetCore.OpenApi.Extensions
{
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
}