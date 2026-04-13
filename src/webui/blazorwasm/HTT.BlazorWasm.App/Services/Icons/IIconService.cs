using HTT.BlazorWasm.App.Models.Icons;
using HTT.BlazorWasm.App.Models.Enums;

namespace HTT.BlazorWasm.App.Services.Icons
{
    public interface IIconService
    {
        Task<(IEnumerable<IconInfo> Items, int TotalCount)> SearchAsync(
            string? searchTerm, 
            IconSet iconSet = IconSet.All, 
            string? category = null, 
            int page = 0, 
            int pageSize = 50);

        Task<IconInfo?> GetIconAsync(string fullKey);
        
        Task<IEnumerable<string>> GetAllCategoriesAsync(IconSet iconSet = IconSet.All);
        
        Task<IEnumerable<CategoryInfo>> GetAllCategoryInfoAsync(IconSet iconSet = IconSet.All);

        Task AddToRecentlyUsedAsync(IconInfo icon);
        
        Task<IEnumerable<IconInfo>> GetRecentlyUsedAsync(int count = 20);
    }
}
