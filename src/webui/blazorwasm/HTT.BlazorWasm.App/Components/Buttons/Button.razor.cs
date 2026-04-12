using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HTT.BlazorWasm.App.Components.Buttons
{
    public partial class Button : BlazorBaseComponent
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public CBVariantType Variant { get; set; } = CBVariantType.Primary;
        [Parameter] public CBSizeType Size { get; set; } = CBSizeType.Md;
        [Parameter] public CBType Type { get; set; } = CBType.Button;
        [Parameter] public string? Href { get; set; }
        [Parameter] public string? Icon { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string? Id { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Class { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        private string ComputeClasses()
        {
            var list = new List<string> { "btn-custom" };
            list.Add($"btn-custom--{Variant.ToString().ToLowerInvariant()}");
            list.Add($"btn-custom--{Size.ToString().ToLowerInvariant()}");
            if (IsLoading) list.Add("btn-custom--loading");
            return string.Join(" ", list);
        }

        private string GetButtonType() => Type.ToString().ToLowerInvariant();
    }
}