namespace AssetStore.ViewModels.Assets;

public class AssetCatalogViewModel
{
    public IReadOnlyList<AssetListItemViewModel> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }

    public bool HasPreviousPage { get; init; }

    public bool HasNextPage { get; init; }

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; init; } = [];

    public int? SelectedCategoryId { get; init; }

    public decimal? SelectedMaxPrice { get; init; }
}

public class AssetListItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string CreatorName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }

    public string PriceDisplay => Price == 0 ? "Free" : $"{Price:F2} PLN";

    public string ShortDescription => Description.Length > 120
        ? $"{Description[..120]}..."
        : Description;
}

