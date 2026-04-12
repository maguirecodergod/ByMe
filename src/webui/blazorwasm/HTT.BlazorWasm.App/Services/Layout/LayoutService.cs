using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using HTT.BlazorWasm.App.Models.Enums;
using HTT.BlazorWasm.App.Models;

namespace HTT.BlazorWasm.App.Services.Layout
{
    public class LayoutService : IDisposable
    {
        private readonly IJSRuntime _js;
        private readonly NavigationManager _navigationManager;
        private CAppThemeType _theme = CAppThemeType.System;
        private string _brand = "default";
        private CSidebarCollapseType _sidebarType = CSidebarCollapseType.Expanded;
        private CSidebarPlacementType _sidebarPlacement = CSidebarPlacementType.Left;
        private int _sidebarWidth = 280; // Default width in pixels
        private string _title = "Dashboard";
        private RenderFragment? _breadcrumbsFragment;
        private List<BreadcrumbItem> _breadcrumbItems = new();
        private List<CNavigationItem> _navigationItems = new();
        private DotNetObjectReference<LayoutService>? _dotNetRef;

        public event Action? OnLayoutChanged;

        public LayoutService(IJSRuntime js, NavigationManager navigationManager)
        {
            _js = js;
            _navigationManager = navigationManager;
            _navigationManager.LocationChanged += HandleLocationChanged;
            InitializeDefaultNavigation();
        }

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            NotifyChanged();
        }

        public CAppThemeType Theme => _theme;
        public string Brand => _brand;
        public CSidebarCollapseType SidebarType => _sidebarType;
        public CSidebarPlacementType SidebarPlacement => _sidebarPlacement;
        public int SidebarWidth => _sidebarWidth;
        public string Title => _title;
        public RenderFragment? BreadcrumbsFragment => _breadcrumbsFragment;
        public List<BreadcrumbItem> BreadcrumbItems => _breadcrumbItems;
        public List<CNavigationItem> NavigationItems => _navigationItems;

        public void SetPageHeader(string title, List<BreadcrumbItem>? items = null, RenderFragment? fragment = null)
        {
            _title = title;
            _breadcrumbItems = items ?? new List<BreadcrumbItem>();
            _breadcrumbsFragment = fragment;
            NotifyChanged();
        }

        public async Task InitializeAsync()
        {
            var savedTheme = await _js.InvokeAsync<string>("localStorage.getItem", "htt-theme");
            if (!string.IsNullOrEmpty(savedTheme))
            {
                if (Enum.TryParse<CAppThemeType>(savedTheme, true, out var theme))
                    _theme = theme;
            }

            var savedBrand = await _js.InvokeAsync<string>("localStorage.getItem", "htt-brand");
            if (!string.IsNullOrEmpty(savedBrand)) _brand = savedBrand;

            _dotNetRef = DotNetObjectReference.Create(this);
            await _js.InvokeVoidAsync("themeHelper.watchSystemTheme", _dotNetRef);
            await _js.InvokeVoidAsync("sidebarHelper.init", _dotNetRef);

            await UpdateAttributes();
            NotifyChanged();
        }

        private void InitializeDefaultNavigation()
        {
            _navigationItems = new List<CNavigationItem>
            {
                new CNavigationItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "" },
                new CNavigationItem { Type = CNavigationItemType.Header, Title = "General" },
                new CNavigationItem { Title = "Counter", Icon = "bi bi-plus-circle", Url = "counter" },
                new CNavigationItem { Title = "Weather", Icon = "bi bi-cloud-sun", Url = "weather" },
                new CNavigationItem { Type = CNavigationItemType.Divider },
                new CNavigationItem { Type = CNavigationItemType.Header, Title = "Enterprise" },
                new CNavigationItem { 
                    Title = "Admin Tools", 
                    Icon = "bi bi-shield-lock", 
                    Type = CNavigationItemType.Group,
                    Children = new List<CNavigationItem> 
                    {
                        new CNavigationItem { Title = "User Management", Icon = "bi bi-people", Url = "admin/users" },
                        new CNavigationItem { Title = "Roles", Icon = "bi bi-node-plus", Url = "admin/roles" }
                    }
                }
            };
        }

        public void SetSidebarPlacement(CSidebarPlacementType placement)
        {
            _sidebarPlacement = placement;
            NotifyChanged();
        }

        [JSInvokable]
        public async Task OnSystemThemeChanged(bool isDark)
        {
            if (_theme == CAppThemeType.System)
            {
                await UpdateAttributes();
                NotifyChanged();
            }
        }

        [JSInvokable]
        public void SetSidebarWidth(int width)
        {
            _sidebarWidth = Math.Clamp(width, 180, 600);
            NotifyChanged();
        }

        public async Task ToggleTheme()
        {
            _theme = _theme switch
            {
                CAppThemeType.System => CAppThemeType.Light,
                CAppThemeType.Light => CAppThemeType.Dark,
                _ => CAppThemeType.System
            };

            await _js.InvokeVoidAsync("localStorage.setItem", "htt-theme", _theme.ToString());
            await UpdateAttributes();
            NotifyChanged();
        }

        public async Task ToggleBrand()
        {
            _brand = _brand switch
            {
                "default" => "lha",
                "lha" => "clientA",
                _ => "default"
            };
            await _js.InvokeVoidAsync("localStorage.setItem", "htt-brand", _brand);
            await UpdateAttributes();
            NotifyChanged();
        }

        public void SetSidebarType(CSidebarCollapseType type)
        {
            _sidebarType = type;
            NotifyChanged();
        }

        public void ToggleSidebar()
        {
            _sidebarType = _sidebarType switch
            {
                CSidebarCollapseType.Expanded => CSidebarCollapseType.Collapsed,
                CSidebarCollapseType.Collapsed => CSidebarCollapseType.Hidden,
                _ => CSidebarCollapseType.Expanded
            };
            NotifyChanged();
        }

        private async Task UpdateAttributes()
        {
            bool isActuallyDark = false;
            if (_theme == CAppThemeType.System)
            {
                isActuallyDark = await _js.InvokeAsync<bool>("themeHelper.isDarkMode");
            }
            else
            {
                isActuallyDark = (_theme == CAppThemeType.Dark);
            }

            var themeValue = isActuallyDark ? "dark" : "light";
            var brandValue = _brand == "default" ? null : _brand;
            
            await _js.InvokeVoidAsync("themeHelper.setAttributes", themeValue, brandValue);
        }

        private void NotifyChanged() => OnLayoutChanged?.Invoke();

        public void Dispose()
        {
            _navigationManager.LocationChanged -= HandleLocationChanged;
            _dotNetRef?.Dispose();
        }
    }
}
