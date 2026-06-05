namespace AssetStore.ViewModels.Home;

public class HomeIndexViewModel
{
    public IReadOnlyList<FeaturedAssetViewModel> LatestAssets { get; init; } = [];
}

public class FeaturedAssetViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string PriceDisplay => Price == 0 ? "Free" : $"{Price:F2} PLN";
}

