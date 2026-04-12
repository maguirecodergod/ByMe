using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Internal;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;

namespace HTT.AspNetCore.OpenApi.Providers
{
    /// <summary>Registers the Scalar interactive API reference UI.</summary>
    internal sealed class ScalarDocsUiProvider : IApiDocsUiProvider
    {
        public CApiDocsUiProvider ProviderType => CApiDocsUiProvider.Scalar;

        public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
        {
            app.MapScalarApiReference(
                OpenApiRouteHelper.UiEndpoint(options.ScalarRoutePrefix),
                scalar =>
                {
                    scalar.Title = options.Title;
                    // scalar.EndpointPathPrefix = $"{OpenApiRouteHelper.UiEndpoint(options.ScalarRoutePrefix)}/{{documentName}}";
                    scalar.OpenApiRoutePattern = OpenApiRouteHelper.JsonEndpointPattern(options.OpenApiRoutePrefix);

                    scalar.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                    
                });
        }
    }
}