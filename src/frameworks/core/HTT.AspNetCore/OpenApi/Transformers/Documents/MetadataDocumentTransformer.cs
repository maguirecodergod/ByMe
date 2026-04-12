using HTT.AspNetCore.OpenApi.Contracts;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace HTT.AspNetCore.OpenApi.Transformers.Documents
{
    /// <summary>
    /// Populates the OpenAPI <c>info</c> object from <see cref="HttOpenApiOptions"/>:
    /// title, description, version, contact, license, terms of service,
    /// external docs, and custom metadata extensions.
    /// </summary>
    internal sealed class MetadataDocumentTransformer(IOptions<HttOpenApiOptions> options)
        : IOpenApiDocumentTransformer
    {
        private readonly HttOpenApiOptions _options = options.Value;

        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            // Per-document override when versioning is used
            var versionDoc = _options.Documents
                .FirstOrDefault(d => d.Name == context.DocumentName);

            document.Info ??= new OpenApiInfo();
            document.Info.Title = versionDoc?.Title ?? _options.Title;
            document.Info.Description = versionDoc?.Description ?? _options.Description;
            document.Info.Version = context.DocumentName;

            if (_options.TermsOfServiceUrl is not null)
                document.Info.TermsOfService = new Uri(_options.TermsOfServiceUrl);

            if (_options.ContactName is not null
                || _options.ContactEmail is not null
                || _options.ContactUrl is not null)
            {
                document.Info.Contact = new OpenApiContact
                {
                    Name = _options.ContactName,
                    Email = _options.ContactEmail,
                    Url = _options.ContactUrl is not null ? new Uri(_options.ContactUrl) : null
                };
            }

            if (_options.LicenseName is not null)
            {
                document.Info.License = new OpenApiLicense
                {
                    Name = _options.LicenseName,
                    Url = _options.LicenseUrl is not null ? new Uri(_options.LicenseUrl) : null
                };
            }

            if (_options.ExternalDocsUrl is not null)
            {
                document.ExternalDocs = new OpenApiExternalDocs
                {
                    Url = new Uri(_options.ExternalDocsUrl),
                    Description = _options.ExternalDocsDescription
                };
            }

            // Inject custom key-value metadata as OpenAPI extensions (x-*)
            document.Info.Extensions ??= new Dictionary<string, IOpenApiExtension>();

            foreach (var (key, value) in _options.Metadata)
            {
                document.Info.Extensions[$"x-{key}"] = new OpenApiAnyExtension(value);
            }

            return Task.CompletedTask;
        }
    }

    internal sealed class OpenApiAnyExtension(object? value) : IOpenApiExtension
    {
        private readonly object? _value = value;

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            switch (_value)
            {
                case null:
                    writer.WriteNull();
                    break;
                case string s:
                    writer.WriteValue(s);
                    break;
                case int i:
                    writer.WriteValue(i);
                    break;
                case long l:
                    writer.WriteValue(l);
                    break;
                case bool b:
                    writer.WriteValue(b);
                    break;
                case double d:
                    writer.WriteValue(d);
                    break;
                default:
                    writer.WriteValue(_value?.ToString() ?? string.Empty);
                    break;
            }
        }
    }
}