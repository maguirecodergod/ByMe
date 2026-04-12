using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HTT.AspNetCore.OpenApi.Providers
{
    /// <summary>
    /// Registers the ReDoc three-panel documentation UI.
    /// When multiple documents are configured, each gets its own route:
    /// <c>/redoc/v1</c>, <c>/redoc/v2</c>, etc.
    /// </summary>
    internal sealed class ReDocDocsUiProvider : IApiDocsUiProvider
    {
        public CApiDocsUiProvider ProviderType => CApiDocsUiProvider.ReDoc;

        public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
        {
            var multipleDocuments = documentNames.Count > 1;
            var reDocRoot = $"/{options.ReDocRoutePrefix}";

            if (multipleDocuments)
            {
                var defaultDocument = documentNames[0];
                app.MapGet(reDocRoot, () => Results.Redirect($"{reDocRoot}/{defaultDocument}", permanent: false));
            }

            foreach (var name in documentNames)
            {
                // Single doc → /redoc   |   Multiple docs → /redoc/v1, /redoc/v2, …
                var routePrefix = multipleDocuments
                    ? $"{options.ReDocRoutePrefix}/{name}"
                    : options.ReDocRoutePrefix;

                app.UseReDoc(redoc =>
                {
                    redoc.RoutePrefix = routePrefix;
                    redoc.SpecUrl = OpenApiRouteHelper.JsonEndpoint(options.OpenApiRoutePrefix, name);
                    redoc.DocumentTitle = multipleDocuments
                        ? $"{options.Title} — {name}"
                        : options.Title;
                });
            }
        }
    }
}