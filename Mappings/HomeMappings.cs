using AssetStore.Dto.Assets;
using AssetStore.ViewModels.Home;

namespace AssetStore.Mappings;

public static class HomeMappings
{
    public static HomeIndexViewModel ToViewModel(IReadOnlyList<AssetListItemDto> assets) => new()
    {
        LatestAssets = assets.Select(a => new FeaturedAssetViewModel
        {
            Id = a.Id,
            Title = a.Title,
            CategoryName = a.CategoryName,
            Price = a.Price
        }).ToList()
    };
}

