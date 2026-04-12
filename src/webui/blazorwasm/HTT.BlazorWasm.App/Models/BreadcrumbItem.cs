namespace HTT.BlazorWasm.App.Models
{
    public class BreadcrumbItem
    {
        public string Label { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
    }
}
