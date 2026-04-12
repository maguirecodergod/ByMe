namespace HTT.AspNetCore.OpenApi.Contracts
{
    public enum CApiDocsUiProvider
    {
        /// <summary>
        /// No UI rendered — only the raw OpenAPI JSON endpoint is available.
        /// </summary>
        None = 0,

        /// <summary>
        /// Scalar — modern, fast, and visually rich API reference.
        /// Route: <c>/{ScalarRoutePrefix}</c>.
        /// </summary>
        Scalar = 1,

        /// <summary>
        /// Swagger UI (Swashbuckle) — classic, widely adopted interactive docs.
        /// Route: <c>/{SwaggerUiRoutePrefix}</c>.
        /// </summary>
        SwaggerUi = 2,

        /// <summary>
        /// ReDoc — clean, responsive three-panel API documentation.
        /// Route: <c>/{ReDocRoutePrefix}</c>.
        /// </summary>
        ReDoc = 3,

        /// <summary>
        /// All available UI providers rendered simultaneously
        /// (Scalar + Swagger UI + ReDoc).
        /// </summary>
        All = 99
    }
}