using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;
using HTT.BlazorWasm.App.Services.Icons.Providers;

namespace HTT.BlazorWasm.App.Services.Icons
{
    public sealed class IconService : IIconService
    {
        private readonly IEnumerable<IIconProvider> _providers;
        private readonly List<IconInfo> _recentlyUsed = new();
        private const int MaxRecentCount = 50;

        public IconService(IEnumerable<IIconProvider> providers)
        {
            _providers = providers;
        }

        public async Task<(IEnumerable<IconInfo> Items, int TotalCount)> SearchAsync(
            string? searchTerm, 
            IconSet iconSet = IconSet.All, 
            string? category = null, 
            int page = 0, 
            int pageSize = 50)
        {
            var selectedProviders = GetProvidersForSet(iconSet);
            var items = new List<IconInfo>();
            int totalCount = 0;

            foreach (var provider in selectedProviders)
            {
                var result = await provider.GetIconsAsync(searchTerm, category, page * pageSize, pageSize);
                totalCount += result.TotalItemCount;
                items.AddRange(result.Items);
            }

            return (items, totalCount);
        }

        public async Task<IconInfo?> GetIconAsync(string fullKey)
        {
            if (string.IsNullOrEmpty(fullKey) || !fullKey.Contains(':')) return null;

            var parts = fullKey.Split(':', 2);
            var providerName = parts[0];
            var key = parts[1];

            var provider = _providers.FirstOrDefault(p => p.Name == providerName);
            return provider != null ? await provider.GetIconByKeyAsync(key) : null;
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync(IconSet iconSet = IconSet.All)
        {
            var providers = GetProvidersForSet(iconSet);
            var categories = new HashSet<string>();

            foreach (var provider in providers)
            {
                var cats = await provider.GetCategoriesAsync();
                foreach (var cat in cats) categories.Add(cat);
            }

            return categories.OrderBy(c => c);
        }

        public async Task<IEnumerable<CategoryInfo>> GetAllCategoryInfoAsync(IconSet iconSet = IconSet.All)
        {
            var providers = GetProvidersForSet(iconSet);
            var info = new List<CategoryInfo>();

            foreach (var provider in providers)
            {
                var providerInfo = await provider.GetCategoryInfoAsync();
                info.AddRange(providerInfo);
            }

            return info.GroupBy(c => c.Name)
                       .Select(g => g.First())
                       .OrderBy(c => c.Name);
        }

        public Task AddToRecentlyUsedAsync(IconInfo icon)
        {
            _recentlyUsed.RemoveAll(i => i.FullKey == icon.FullKey);
            _recentlyUsed.Insert(0, icon);

            if (_recentlyUsed.Count > MaxRecentCount)
            {
                _recentlyUsed.RemoveAt(_recentlyUsed.Count - 1);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<IconInfo>> GetRecentlyUsedAsync(int count = 20)
        {
            return Task.FromResult(_recentlyUsed.Take(count));
        }

        private IEnumerable<IIconProvider> GetProvidersForSet(IconSet set)
        {
            if (set == IconSet.All) return _providers;
            return _providers.Where(p => p.SupportedSet == set || p.SupportedSet == IconSet.All);
        }
    }
}
