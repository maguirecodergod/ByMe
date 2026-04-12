namespace HTT.BlazorWasm.App.Models
{
    public enum CNavigationItemType
    {
        Link,
        Header,
        Divider,
        Group
    }

    public class CNavigationItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public CNavigationItemType Type { get; set; } = CNavigationItemType.Link;
        public List<CNavigationItem> Children { get; set; } = new();
        
        // Authorization
        public List<string>? Roles { get; set; }
        public List<string>? Permissions { get; set; }
        
        // Metadata
        public string? BadgeText { get; set; }
        public string? BadgeClass { get; set; }
        public bool IsExpanded { get; set; }
    }
}
