using Microsoft.JSInterop;
using HTT.BlazorWasm.App.Models.Enums;
using Microsoft.AspNetCore.Components.Web;
using HTT.BlazorWasm.App.Components;

namespace HTT.BlazorWasm.App.Layout
{
    public partial class NavMenu : BlazorLayoutComponent
    {

        private bool IsVertical => LayoutService.SidebarPlacement == CSidebarPlacementType.Left
            || LayoutService.SidebarPlacement == CSidebarPlacementType.Right;

        private string GetSidebarClasses()
        {
            var classes = new List<string> { "enterprise-sidebar" };

            classes.Add($"placement-{LayoutService.SidebarPlacement.ToString().ToLower()}");

            if (LayoutService.SidebarType == CSidebarCollapseType.Collapsed)
                classes.Add("is-collapsed");
            else if (LayoutService.SidebarType == CSidebarCollapseType.Hidden)
                classes.Add("is-hidden");

            return string.Join(" ", classes);
        }

        private string GetSidebarStyles()
        {
            if (LayoutService.SidebarType == CSidebarCollapseType.Collapsed) return string.Empty;
            if (LayoutService.SidebarType == CSidebarCollapseType.Hidden) return string.Empty;

            if (IsVertical)
                return $"width: {LayoutService.SidebarWidth}px;";

            return "";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LayoutService.OnLayoutChanged += StateHasChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            LayoutService.OnLayoutChanged -= StateHasChanged;
        }

        private async Task OnStartResize(MouseEventArgs e)
        {
            if (e.Button != 0) return;
            await JS.InvokeVoidAsync("sidebarHelper.startResizing");
        }
    }
}