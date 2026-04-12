using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Internal;
using Microsoft.AspNetCore.Builder;

namespace HTT.AspNetCore.OpenApi.Providers
{
    /// <summary>Registers the Swagger UI (Swashbuckle) interactive docs.</summary>
    internal sealed class SwaggerDocsUiProvider : IApiDocsUiProvider
    {
        public CApiDocsUiProvider ProviderType => CApiDocsUiProvider.SwaggerUi;

        public void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames)
        {
            app.UseSwaggerUI(ui =>
            {
                ui.RoutePrefix = options.SwaggerUiRoutePrefix;

                foreach (var name in documentNames)
                {
                    ui.SwaggerEndpoint(
                        url: OpenApiRouteHelper.JsonEndpoint(options.OpenApiRoutePrefix, name),
                        name: name);
                }

                ui.DocumentTitle = options.Title;
            });
        }
    }
}