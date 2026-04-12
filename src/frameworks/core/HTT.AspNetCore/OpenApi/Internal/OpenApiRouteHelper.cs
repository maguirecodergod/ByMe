namespace HTT.AspNetCore.OpenApi.Internal
{
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
}