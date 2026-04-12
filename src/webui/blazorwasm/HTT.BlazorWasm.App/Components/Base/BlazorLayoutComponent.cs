using HTT.BlazorWasm.App.Services.Layout;
using HTT.BlazorWasm.App.Services.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace HTT.BlazorWasm.App.Components
{
    public abstract class BlazorLayoutComponent : LayoutComponentBase
    {
        [Inject] protected LayoutService LayoutService { get; set; } = default!;
        [Inject] protected ILogger<BlazorBaseComponent> Logger { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected IStringLocalizer<BlazorBaseComponent> Localizer { get; set; } = default!;
        [Inject] protected IToaster Toaster { get; set; } = default!;
        [Inject] protected LocalizationService LocalizationService { get; set; } = default!;

        protected override void OnInitialized()
        {
            LocalizationService.OnCultureChanged += HandleCultureChanged;
        }

        protected virtual void HandleCultureChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose()
        {
            LocalizationService.OnCultureChanged -= HandleCultureChanged;
            GC.SuppressFinalize(this);
        }
    }
}