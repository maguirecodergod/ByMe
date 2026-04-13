using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace HTT.BlazorWasm.App.Services.Icons.Providers
{
    public interface IIconProvider
    {
        string Name { get; }
        IconSet SupportedSet { get; }
        
        Task InitializeAsync();
        
        Task<ItemsProviderResult<IconInfo>> GetIconsAsync(
            string? searchTerm, 
            string? category, 
            int skip, 
            int take);
            
        Task<IconInfo?> GetIconByKeyAsync(string key);
        
        Task<IEnumerable<string>> GetCategoriesAsync();
        
        Task<IEnumerable<CategoryInfo>> GetCategoryInfoAsync();
    }
}
