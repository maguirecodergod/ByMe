using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HTT.AspNetCore.OpenApi.Transformers.Operations
{
    /// <summary>
    /// Injects standard HTTP error responses (400, 401, 403, 500) with
    /// <c>application/problem+json</c> content into every operation that
    /// does not already define them.
    /// </summary>
    internal sealed class DefaultResponseOperationTransformer : IOpenApiOperationTransformer
    {
        private static readonly IReadOnlyDictionary<string, OpenApiResponse> DefaultErrorResponses =
            new Dictionary<string, OpenApiResponse>
            {
                ["400"] = Problem("Bad Request", "Invalid input or validation failure."),
                ["401"] = Problem("Unauthorized", "Missing or invalid authentication credentials."),
                ["403"] = Problem("Forbidden", "Insufficient permissions to perform this action."),
                ["500"] = Problem("Internal Server Error", "An unexpected error occurred on the server.")
            };

        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            foreach (var (statusCode, response) in DefaultErrorResponses)
            {
                // Respect any response the developer has explicitly declared
                if (operation.Responses != null)
                {
                    operation.Responses.TryAdd(statusCode, response);
                }
            }

            return Task.CompletedTask;
        }

        // ─── Private ──────────────────────────────────────────────────────

        private static OpenApiResponse Problem(string description, string detail) => new()
        {
            Description = $"{description} — {detail}",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    // Keep media type declared without a hard schema reference,
                    // because a shared ProblemDetails component may not exist
                    // in all generated OpenAPI documents.
                }
            }
        };
    }
}