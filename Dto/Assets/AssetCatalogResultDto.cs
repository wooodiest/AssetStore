using AssetStore.Dto.Categories;
using AssetStore.Models.Common;

namespace AssetStore.Dto.Assets;

public class AssetCatalogResultDto
{
    public PagedResult<AssetListItemDto> Assets { get; init; } = new();

    public IReadOnlyList<CategoryOptionDto> Categories { get; init; } = [];

    public int? SelectedCategoryId { get; init; }

    public decimal? SelectedMaxPrice { get; init; }
}

