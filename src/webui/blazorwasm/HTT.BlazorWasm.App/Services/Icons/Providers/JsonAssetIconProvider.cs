using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Net.Http.Json;

namespace HTT.BlazorWasm.App.Services.Icons.Providers
{
    public sealed class JsonAssetIconProvider : IIconProvider, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private MasterData? _cache;
        private ILookup<string, IconInfo>? _categoryIcons;
        private bool _isInitialized;

        public JsonAssetIconProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string Name => "Enterprise Hub";
        public IconSet SupportedSet => IconSet.All;

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await _lock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                // Load the centralized enterprise metadata (4,500+ items)
                _cache = await _httpClient.GetFromJsonAsync<MasterData>("assets/icons/enterprise-metadata.json");
                
                if (_cache != null)
                {
                    var flatIcons = new List<IconInfo>();
                    foreach (var group in _cache.Icons)
                    {
                        var categoryName = group.Key;
                        var categoryIcons = group.Value;
                        var catDto = _cache.Categories.FirstOrDefault(c => c.Name == categoryName);

                        foreach (var dto in categoryIcons)
                        {
                            var displayType = dto.Type switch
                            {
                                "Emoji" => IconDisplayType.Emoji,
                                "Css" => IconDisplayType.Css,
                                "Svg" => IconDisplayType.Svg,
                                _ => IconDisplayType.Emoji
                            };

                            flatIcons.Add(new IconInfo(
                                Name: dto.Name,
                                Key: dto.Key,
                                Content: dto.Content,
                                DisplayType: displayType,
                                Category: categoryName,
                                CategoryIcon: catDto?.Icon ?? "❓",
                                ProviderName: Name
                            ));
                        }
                    }

                    _categoryIcons = flatIcons.ToLookup(i => i.Category);
                }

                _isInitialized = true;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task EnsureInitializedAsync() => await InitializeAsync();

        public async Task<ItemsProviderResult<IconInfo>> GetIconsAsync(string? searchTerm, string? category, int skip, int take)
        {
            await EnsureInitializedAsync();
            if (_categoryIcons == null) return new ItemsProviderResult<IconInfo>(Enumerable.Empty<IconInfo>(), 0);

            IEnumerable<IconInfo> query = string.IsNullOrEmpty(category) 
                ? _categoryIcons.SelectMany(g => g) 
                : _categoryIcons[category];

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i => i.SearchTerms.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = query.Count();
            var items = query.Skip(skip).Take(take).ToList();

            return new ItemsProviderResult<IconInfo>(items, totalCount);
        }

        public async Task<IconInfo?> GetIconByKeyAsync(string key)
        {
            await EnsureInitializedAsync();
            return _categoryIcons?.SelectMany(g => g).FirstOrDefault(i => i.Key == key);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            await EnsureInitializedAsync();
            return _cache?.Categories.Select(c => c.Name) ?? Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<CategoryInfo>> GetCategoryInfoAsync()
        {
            await EnsureInitializedAsync();
            return _cache?.Categories.Select(c => new CategoryInfo(c.Name, c.Icon, IconDisplayType.Emoji)) 
                   ?? Enumerable.Empty<CategoryInfo>();
        }

        public void Dispose() => _lock.Dispose();

        #region DTOs
        private record MasterData(List<CategoryDto> Categories, Dictionary<string, List<IconDto>> Icons);
        private record CategoryDto(string Name, string Icon, string Type);
        private record IconDto(string Name, string Key, string Content, string Type);
        #endregion
    }
}
