using HTT.AspNetCore.OpenApi.Contracts;
using Microsoft.OpenApi;

namespace HTT.AspNetCore.OpenApi.Internal
{
    /// <summary>Maps <see cref="ApiSecurityScheme"/> config objects to <see cref="OpenApiSecurityScheme"/>.</summary>
    internal static class SecuritySchemeMapper
    {
        public static OpenApiSecurityScheme Map(ApiSecurityScheme source)
        {
            var type = source.Type.ToUpperInvariant() switch
            {
                "HTTP" => SecuritySchemeType.Http,
                "APIKEY" => SecuritySchemeType.ApiKey,
                "OAUTH2" => SecuritySchemeType.OAuth2,
                "OPENIDCONNECT" => SecuritySchemeType.OpenIdConnect,
                _ => SecuritySchemeType.Http
            };

            var location = source.Location.ToUpperInvariant() switch
            {
                "QUERY" => ParameterLocation.Query,
                "COOKIE" => ParameterLocation.Cookie,
                _ => ParameterLocation.Header
            };

            var scheme = new OpenApiSecurityScheme
            {
                Name = source.ParameterName ?? source.Name,
                Type = type,
                In = location,
                Scheme = source.Scheme,
                BearerFormat = source.BearerFormat,
                Description = source.Description
            };

            if (type == SecuritySchemeType.OpenIdConnect && source.OpenIdConnectUrl is not null)
            {
                scheme.OpenIdConnectUrl = new Uri(source.OpenIdConnectUrl);
            }

            if (type == SecuritySchemeType.OAuth2)
            {
                scheme.Flows = BuildOAuthFlows(source);
            }

            return scheme;
        }

        // ─── Private ──────────────────────────────────────────────────────

        private static OpenApiOAuthFlows BuildOAuthFlows(ApiSecurityScheme source)
        {
            var scopes = source.Scopes ?? [];
            var hasAuthUrl = source.AuthorizationUrl is not null;
            var hasTokenUrl = source.TokenUrl is not null;
            var refreshUrl = source.RefreshUrl is not null ? new Uri(source.RefreshUrl) : null;

            return new OpenApiOAuthFlows
            {
                // Both URLs → Authorization Code flow
                AuthorizationCode = hasAuthUrl && hasTokenUrl
                    ? new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(source.AuthorizationUrl!),
                        TokenUrl = new Uri(source.TokenUrl!),
                        RefreshUrl = refreshUrl,
                        Scopes = scopes
                    }
                    : null,

                // Token URL only → Client Credentials flow
                ClientCredentials = hasTokenUrl && !hasAuthUrl
                    ? new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri(source.TokenUrl!),
                        RefreshUrl = refreshUrl,
                        Scopes = scopes
                    }
                    : null,

                // Auth URL only → Implicit flow
                Implicit = hasAuthUrl && !hasTokenUrl
                    ? new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(source.AuthorizationUrl!),
                        RefreshUrl = refreshUrl,
                        Scopes = scopes
                    }
                    : null
            };
        }
    }
}