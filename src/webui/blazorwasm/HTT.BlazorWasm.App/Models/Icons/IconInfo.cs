using HTT.BlazorWasm.App.Models.Enums;
using System.Text.Json.Serialization;

namespace HTT.BlazorWasm.App.Models.Icons
{
    /// <summary>
    /// Represents a production-grade icon/emoji record.
    /// Uses C# records for high-performance immutability.
    /// </summary>
    public record IconInfo(
        string Name,
        string Key,
        string Content,
        IconDisplayType DisplayType,
        string Category,
        string CategoryIcon,
        string ProviderName)
    {
        // Computed unique identifier for storage and state matching
        [JsonIgnore]
        public string FullKey => $"{ProviderName}:{Key}";

        // Computed property for enterprise search indexing
        [JsonIgnore]
        public string SearchTerms => $"{Name} {Key} {Category}".ToLowerInvariant();

        // Type helpers
        [JsonIgnore]
        public bool IsSvg => DisplayType == IconDisplayType.Svg;
        [JsonIgnore]
        public bool IsEmoji => DisplayType == IconDisplayType.Emoji;
        [JsonIgnore]
        public bool IsCss => DisplayType == IconDisplayType.Css;
    }

    public record CategoryInfo(
        string Name,
        string Icon,
        IconDisplayType IconDisplayType);
}
