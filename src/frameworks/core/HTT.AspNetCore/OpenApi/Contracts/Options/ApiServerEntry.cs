namespace HTT.AspNetCore.OpenApi.Contracts
{
    /// <summary>Defines a server entry in the OpenAPI document.</summary>
    public sealed class ApiServerEntry
    {
        /// <summary>Server URL (e.g. <c>"https://api.example.com"</c>).</summary>
        public required string Url { get; set; }

        /// <summary>Description of the server (e.g. <c>"Production"</c>, <c>"Staging"</c>).</summary>
        public string? Description { get; set; }
    }
}