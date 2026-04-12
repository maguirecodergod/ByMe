using HTT.BlazorWasm.App.Components;
using HTT.BlazorWasm.App.Models.Enums;
using HTT.BlazorWasm.App.Services.Layout;

namespace HTT.BlazorWasm.App.Layout
{
    public partial class MainLayout : BlazorLayoutComponent
    {
        private string GetLayoutClasses()
        {
            var classes = new List<string> { "page-layout" };
            classes.Add($"layout-sidebar-{LayoutService.SidebarPlacement.ToString().ToLower()}");
            classes.Add($"sidebar-state-{LayoutService.SidebarType.ToString().ToLower()}");
            return string.Join(" ", classes);
        }

        private string GetLayoutStyles()
        {
            if (LayoutService.SidebarPlacement == CSidebarPlacementType.Left
                || LayoutService.SidebarPlacement == CSidebarPlacementType.Right)
            {
                var width = LayoutService.SidebarType == CSidebarCollapseType.Collapsed ? "64px"
                    : LayoutService.SidebarType == CSidebarCollapseType.Hidden ? "0px"
                        : $"{LayoutService.SidebarWidth}px";
                return $"--sidebar-current-width: {width};";
            }
            return string.Empty;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LayoutService.InitializeAsync();
                StateHasChanged();
            }
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
    }
}