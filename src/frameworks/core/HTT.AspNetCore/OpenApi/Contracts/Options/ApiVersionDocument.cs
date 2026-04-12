namespace HTT.AspNetCore.OpenApi.Contracts
{
    /// <summary>
    /// Defines a named API version document.
    /// </summary>
    public class ApiVersionDocument
    {
        /// <summary>
        /// Document name used in the route (e.g. <c>"v1"</c>, <c>"v2"</c>).
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Display title for this version.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Description for this version.
        /// </summary>
        public string? Description { get; set; }
    }
}