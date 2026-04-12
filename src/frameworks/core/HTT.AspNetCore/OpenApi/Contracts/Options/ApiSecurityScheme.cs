namespace HTT.AspNetCore.OpenApi.Contracts
{
    /// <summary>
    /// Defines a security scheme for the OpenAPI document.
    /// </summary>
    public sealed class ApiSecurityScheme
    {
        /// <summary>Scheme identifier (e.g. <c>"Bearer"</c>, <c>"ApiKey"</c>).</summary>
        public required string Name { get; set; }

        /// <summary>
        /// Scheme type: <c>"Http"</c>, <c>"ApiKey"</c>, <c>"OAuth2"</c>, <c>"OpenIdConnect"</c>.
        /// </summary>
        public required string Type { get; set; }

        /// <summary>Where the credential is sent: <c>"Header"</c>, <c>"Query"</c>, <c>"Cookie"</c>.</summary>
        public string Location { get; set; } = "Header";

        /// <summary>Header / query parameter name (for ApiKey schemes).</summary>
        public string? ParameterName { get; set; }

        /// <summary>HTTP scheme (e.g. <c>"bearer"</c> for JWT).</summary>
        public string? Scheme { get; set; }

        /// <summary>Bearer format hint (e.g. <c>"JWT"</c>).</summary>
        public string? BearerFormat { get; set; }

        /// <summary>Description shown in the docs.</summary>
        public string? Description { get; set; }

        /// <summary>OpenID Connect discovery URL.</summary>
        public string? OpenIdConnectUrl { get; set; }

        /// <summary>OAuth2 authorization URL.</summary>
        public string? AuthorizationUrl { get; set; }

        /// <summary>OAuth2 token URL.</summary>
        public string? TokenUrl { get; set; }

        /// <summary>OAuth2 refresh URL.</summary>
        public string? RefreshUrl { get; set; }

        /// <summary>OAuth2 scopes as <c>"scope": "description"</c>.</summary>
        public Dictionary<string, string>? Scopes { get; set; }
    }
}