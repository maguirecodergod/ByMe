using HTT.BlazorWasm.App.Components;

namespace HTT.BlazorWasm.App.Layout
{
    public partial class Topbar : BlazorLayoutComponent
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            LayoutService.OnLayoutChanged += HandleLayoutChanged;
        }

        private void HandleLayoutChanged() => StateHasChanged();

        public override void Dispose()
        {
            base.Dispose();
            LayoutService.OnLayoutChanged -= HandleLayoutChanged;
        }
    }
}