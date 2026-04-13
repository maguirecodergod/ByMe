using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace HTT.BlazorWasm.App.Services.Icons.Providers
{
    /// <summary>
    /// Specialized provider for stress-testing virtualization and search performance.
    /// Simulates 100,000+ icons with categorisation and real-time filtering.
    /// </summary>
    public sealed class LargeSimulatedIconProvider : IIconProvider
    {
        private const int TotalDesiredIcons = 100000;
        private static readonly string[] Categories = { 
            "Smileys", "Nature", "Food", "Activities", "Travel", "Objects", "Symbols", "Flags", 
            "Medical", "Engineering", "Astronomy", "Music", "Logistics", "Law", "Education" 
        };
        private static readonly string[] Emojis = { "😀", "😎", "🚀", "🔥", "🌈", "🍕", "🐶", "💼", "📱", "🏠", "⚡", "❤️", "⚽", "🚗", "💡" };

        private List<IconInfo> _testData = new();
        private bool _isInitialized;

        public string Name => "Stress Test Provider";
        public IconSet SupportedSet => IconSet.All;

        public Task InitializeAsync()
        {
            if (_isInitialized) return Task.CompletedTask;

            // Generate 100,000 unique records in memory for testing
            for (int i = 1; i <= TotalDesiredIcons; i++)
            {
                var category = Categories[i % Categories.Length];
                var emoji = Emojis[i % Emojis.Length];
                
                _testData.Add(new IconInfo(
                    Name: $"Icon #{i} of {category}",
                    Key: $"node_{i}",
                    Content: emoji,
                    DisplayType: IconDisplayType.Emoji,
                    Category: category,
                    CategoryIcon: Emojis[Array.IndexOf(Categories, category) % Emojis.Length],
                    ProviderName: Name
                ));
            }

            _isInitialized = true;
            return Task.CompletedTask;
        }

        public async Task<ItemsProviderResult<IconInfo>> GetIconsAsync(string? searchTerm, string? category, int skip, int take)
        {
            await InitializeAsync();

            IEnumerable<IconInfo> query = _testData;

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var total = query.Count();
            var items = query.Skip(skip).Take(take).ToList();

            return new ItemsProviderResult<IconInfo>(items, total);
        }

        public async Task<IconInfo?> GetIconByKeyAsync(string key)
        {
            await InitializeAsync();
            return _testData.FirstOrDefault(i => i.Key == key);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            await InitializeAsync();
            return Categories;
        }

        public async Task<IEnumerable<CategoryInfo>> GetCategoryInfoAsync()
        {
            await InitializeAsync();
            return Categories.Select(c => new CategoryInfo(
                c, 
                Emojis[Array.IndexOf(Categories, c) % Emojis.Length], 
                IconDisplayType.Emoji));
        }
    }
}
