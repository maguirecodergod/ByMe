using HTT.AspNetCore.OpenApi.Contracts;
using Microsoft.AspNetCore.Builder;

namespace HTT.AspNetCore.OpenApi.Providers
{
    /// <summary>
    /// Contract for registering an API documentation UI into the request pipeline.
    /// Each implementation handles exactly one <see cref="ApiDocsUiProvider"/> value.
    /// </summary>
    public interface IApiDocsUiProvider
    {
        /// <summary>Identifies which <see cref="ApiDocsUiProvider"/> this implementation handles.</summary>
        CApiDocsUiProvider ProviderType { get; }

        /// <summary>Registers routes and/or middleware for this UI onto the application.</summary>
        /// <param name="app">The running web application.</param>
        /// <param name="options">Resolved OpenAPI options.</param>
        /// <param name="documentNames">Ordered list of all active document names.</param>
        void Map(WebApplication app, HttOpenApiOptions options, IReadOnlyList<string> documentNames);
    }
}