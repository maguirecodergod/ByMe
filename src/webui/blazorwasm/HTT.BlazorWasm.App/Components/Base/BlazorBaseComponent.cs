using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using HTT.BlazorWasm.App.Services.Localization;

namespace HTT.BlazorWasm.App.Components.Base
{
    public class BlazorBaseComponent : ComponentBase, IDisposable
    {
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