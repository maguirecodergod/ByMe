using HTT.BlazorWasm.App.Models;
using HTT.BlazorWasm.App.Models.Enums;
using HTT.BlazorWasm.App.Services.Layout;
using Microsoft.AspNetCore.Components;

namespace HTT.BlazorWasm.App.Components.Layout.Sidebar
{
    public partial class SidebarGroup : BlazorBaseComponent
    {
        [Inject] private LayoutService LayoutService { get; set; } = default!;

        [Parameter, EditorRequired] public NavigationItem Item { get; set; } = default!;

        private bool IsExpanded { get; set; }
        private bool IsActive => Item.Children.Any(c => !string.IsNullOrEmpty(c.Url)
            && NavigationManager.ToBaseRelativePath(NavigationManager.Uri).StartsWith(c.Url ?? "NONE"));

        private void Toggle()
        {
            if (LayoutService.SidebarType == CSidebarCollapseType.Collapsed) return;
            IsExpanded = !IsExpanded;
        }

        protected override void OnInitialized()
        {
            // Auto expand if active or was expanded
            IsExpanded = IsActive || Item.IsExpanded;
        }
    }
}