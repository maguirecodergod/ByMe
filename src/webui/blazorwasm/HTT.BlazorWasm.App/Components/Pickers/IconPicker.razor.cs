using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;
using HTT.BlazorWasm.App.Services.Icons;
using Microsoft.JSInterop;
using System.Timers;

namespace HTT.BlazorWasm.App.Components.Pickers
{
    public sealed partial class IconPicker : BlazorBaseComponent, IAsyncDisposable
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public EventCallback<string?> OnChanged { get; set; }
        [Parameter] public IconSet IconSet { get; set; } = IconSet.All;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string Class { get; set; } = string.Empty;

        [Inject] private IIconService IconService { get; set; } = default!;
        // Lưu ý: BlazorBaseComponent đã có JS, không khai báo lại để tránh CS0108

        private string? _searchTerm;
        private string? _selectedCategory;
        private IEnumerable<CategoryInfo> _categories = Enumerable.Empty<CategoryInfo>();
        private bool _isPickerOpen;
        private IconInfo? _selectedIconInfo;
        private System.Timers.Timer? _debounceTimer;
        private ElementReference _searchInput;
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<IconPicker>? _dotNetRef;
        private string _pickerId = $"picker_{Guid.NewGuid():N}";
        private string _triggerId = $"trigger_{Guid.NewGuid():N}";
        private bool _shouldFlipUp;

        private Virtualize<IconRow>? _virtualizeComponent;
        private int _totalRows;
        
        // Chuyển sang protected để Razor có thể thấy chắc chắn
        protected const int IconsPerRow = 8;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _categories = (await IconService.GetAllCategoryInfoAsync(IconSet)).ToList();
            if (string.IsNullOrEmpty(_selectedCategory) && _categories.Any())
            {
                _selectedCategory = _categories.First().Name;
            }

            if (!string.IsNullOrEmpty(Value))
            {
                _selectedIconInfo = await IconService.GetIconAsync(Value);
            }

            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += async (s, e) => await InvokeAsync(RefreshIcons);
        }

        private async Task TogglePicker()
        {
            if (Disabled) return;
            
            _isPickerOpen = !_isPickerOpen;

            if (_isPickerOpen)
            {
                try 
                {
                    // Sử dụng biến JS từ base class
                    _shouldFlipUp = await JS.InvokeAsync<bool>("iconPickerHelper.shouldFlipUp", _triggerId);
                    
                    _dotNetRef ??= DotNetObjectReference.Create(this);
                    
                    // Delay nhỏ để đảm bảo ID tồn tại trong DOM
                    await Task.Delay(1);
                    _jsModule = await JS.InvokeAsync<IJSObjectReference>("iconPickerHelper.addOutsideClickListener", _pickerId, _dotNetRef);
                    
                    if (_searchInput.Context != null) 
                    {
                        await JS.InvokeVoidAsync("iconPickerHelper.focusInput", _searchInput);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"IconPicker JS Error: {ex.Message}");
                    _shouldFlipUp = false;
                }
                
                StateHasChanged();
            }
            else
            {
                await CleanupJs();
            }
        }

        [JSInvokable]
        public async Task ClosePicker()
        {
            _isPickerOpen = false;
            await CleanupJs();
            StateHasChanged();
        }

        private async Task RefreshIcons()
        {
            if (_virtualizeComponent != null)
            {
                await _virtualizeComponent.RefreshDataAsync();
                StateHasChanged();
            }
        }

        private async ValueTask<ItemsProviderResult<IconRow>> LoadIconRows(ItemsProviderRequest request)
        {
            int skipIcons = request.StartIndex * IconsPerRow;
            int takeIcons = request.Count * IconsPerRow;

            var (items, totalIcons) = await IconService.SearchAsync(
                _searchTerm,
                IconSet,
                _selectedCategory,
                skipIcons / takeIcons,
                takeIcons);

            var rows = new List<IconRow>();
            var itemList = items.ToList();
            for (int i = 0; i < itemList.Count; i += IconsPerRow)
            {
                rows.Add(new IconRow { Icons = itemList.Skip(i).Take(IconsPerRow).ToList() });
            }

            _totalRows = (int)Math.Ceiling((double)totalIcons / IconsPerRow);
            return new ItemsProviderResult<IconRow>(rows, _totalRows);
        }

        private void OnSearchInput(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString();
            _debounceTimer?.Stop();
            _debounceTimer?.Start();
        }

        protected async Task ClearSearch()
        {
            _searchTerm = null;
            await RefreshIcons();
        }

        private async Task SelectIcon(IconInfo icon)
        {
            _selectedIconInfo = icon;
            Value = icon.FullKey;
            _isPickerOpen = false;
            await CleanupJs();

            await IconService.AddToRecentlyUsedAsync(icon);
            await ValueChanged.InvokeAsync(Value);
            await OnChanged.InvokeAsync(Value);
            StateHasChanged();
        }

        private async Task SelectCategory(string? category)
        {
            _selectedCategory = category;
            await RefreshIcons();
        }

        private async Task CleanupJs()
        {
            if (_jsModule != null)
            {
                try {
                    await _jsModule.InvokeVoidAsync("dispose");
                } catch { }
                await _jsModule.DisposeAsync();
                _jsModule = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CleanupJs();
            _dotNetRef?.Dispose();
            _debounceTimer?.Dispose();
        }
    }

    public class IconRow
    {
        public List<IconInfo> Icons { get; set; } = new();
    }
}
