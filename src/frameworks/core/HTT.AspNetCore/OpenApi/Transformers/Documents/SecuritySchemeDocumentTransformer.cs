using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Internal;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents
{
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
                if (document.Components.SecuritySchemes != null)
                {
                    document.Components.SecuritySchemes[scheme.Name] = SecuritySchemeMapper.Map(scheme);
                }
            }

            return Task.CompletedTask;
        }
    }
}