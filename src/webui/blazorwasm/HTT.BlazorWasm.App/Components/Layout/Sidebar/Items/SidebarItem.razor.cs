using HTT.BlazorWasm.App.Models;
using HTT.BlazorWasm.App.Services.Layout;
using Microsoft.AspNetCore.Components;

namespace HTT.BlazorWasm.App.Components.Layout.Sidebar
{
    /// <summary>
    /// Sidebar Item Component
    /// </summary>
    public partial class SidebarItem : BlazorBaseComponent
    {
        /// <summary>
        /// Layout Service
        /// </summary>
        [Inject] private LayoutService LayoutService { get; set; } = default!;

        /// <summary>
        /// Navigation Item
        /// </summary>
        [Parameter, EditorRequired] public NavigationItem Item { get; set; } = default!;

        /// <summary>
        /// Check if the item is active
        /// </summary>
        private bool IsActive => !string.IsNullOrEmpty(Item.Url)
            && (Item.Url == string.Empty ? NavigationManager.Uri.TrimEnd('/') == NavigationManager.BaseUri.TrimEnd('/')
                : NavigationManager.Uri.Contains(Item.Url));

        /// <summary>
        /// Check if the item is visible
        /// </summary>
        private bool IsVisible => CheckPermissions();

        /// <summary>
        /// Check permissions
        /// </summary>
        private bool CheckPermissions()
        {
            // In a real app, inject AuthenticationStateProvider and check roles/permissions
            // Here we simulate permitting everything except specific demo cases
            if (Item.Roles != null && Item.Roles.Any())
            {
                // return userRoles.Intersect(Item.Roles).Any();
                Logger.LogInformation("Item {Item} is not visible", Item.Title);
            }
            return true;
        }
    }
}