using HTT.AspNetCore.OpenApi.Contracts;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents
{
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
                    Url = s.Url,
                    Description = s.Description
                })
                .ToList();

            return Task.CompletedTask;
        }
    }
}