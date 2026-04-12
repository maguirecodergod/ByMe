using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.JSInterop;

namespace HTT.BlazorWasm.App.Services.Localization
{
    public class LocalizationService
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient _httpClient;
        private CultureInfo _currentCulture = CultureInfo.CurrentCulture;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>> _cache = new();
        private readonly ConcurrentDictionary<string, Task> _loadingTasks = new();

        public event Action? OnCultureChanged;

        public LocalizationService(IJSRuntime js, HttpClient httpClient)
        {
            _js = js;
            _httpClient = httpClient;
        }

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture.Name != value.Name)
                {
                    _currentCulture = value;
                    CultureInfo.DefaultThreadCurrentCulture = value;
                    CultureInfo.DefaultThreadCurrentUICulture = value;
                    OnCultureChanged?.Invoke();
                }
            }
        }

        public async Task SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            await _js.InvokeVoidAsync("localStorage.setItem", "blazor-culture", cultureName);
            CurrentCulture = culture;
        }

        public void Initialize(string cultureName)
        {
            _currentCulture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;
        }

        public string? GetString(string resourceName, string key)
        {
            var culture = _currentCulture.Name;
            
            if (_cache.TryGetValue(culture, out var resources))
            {
                if (resources.TryGetValue(resourceName, out var values))
                {
                    return values.TryGetValue(key, out var value) ? value : null;
                }
            }

            // Trigger load in background
            _ = LoadResource(culture, resourceName);
            return null;
        }

        private async Task LoadResource(string culture, string resourceName)
        {
            var taskKey = $"{culture}_{resourceName}";
            if (_loadingTasks.ContainsKey(taskKey))
            {
                await _loadingTasks[taskKey];
                return;
            }

            var loadTask = Task.Run(async () =>
            {
                try
                {
                    var path = $"locales/{culture}/{resourceName}.json";
                    var json = await _httpClient.GetStringAsync(path);
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        var cultureCache = _cache.GetOrAdd(culture, _ => new ConcurrentDictionary<string, Dictionary<string, string>>());
                        cultureCache[resourceName] = data;
                        NotifyChanged();
                    }
                }
                catch
                {
                    var cultureCache = _cache.GetOrAdd(culture, _ => new ConcurrentDictionary<string, Dictionary<string, string>>());
                    cultureCache[resourceName] = new Dictionary<string, string>();
                }
                finally
                {
                    _loadingTasks.TryRemove(taskKey, out _);
                }
            });

            _loadingTasks[taskKey] = loadTask;
            await loadTask;
        }

        public void NotifyChanged() => OnCultureChanged?.Invoke();
    }
}
