namespace HTT.AspNetCore.OpenApi.Contracts
{
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
        public CApiDocsUiProvider UiProvider { get; set; } = CApiDocsUiProvider.Scalar;

        // ─── Document metadata ───────────────────────────────────────────

        /// <summary>Service / API title displayed in the docs.</summary>
        public string Title { get; set; } = "HTT API";

        /// <summary>Short description of the service.</summary>
        public string? Description { get; set; }

        /// <summary>API version string used when <see cref="Documents"/> is empty (e.g. <c>"v1"</c>).</summary>
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
        /// Keys are automatically prefixed with <c>x-</c>.
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
        /// Server URLs included in the OpenAPI document.
        /// Useful for gateway or public-facing base URLs.
        /// </summary>
        public List<ApiServerEntry> Servers { get; set; } = [];
    }
}