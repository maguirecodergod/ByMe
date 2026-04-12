using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HTT.BlazorWasm.App.Components
{
    public partial class Tooltip : BlazorBaseComponent
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Title { get; set; }

        private ElementReference containerRef;

        private async Task HandleMouseEnter()
        {
            if (!string.IsNullOrEmpty(Title))
            {
                await JS.InvokeVoidAsync("tooltipHelper.show", containerRef, Title);
            }
        }

        private async Task HandleMouseLeave()
        {
            await JS.InvokeVoidAsync("tooltipHelper.hide");
        }
    }
}