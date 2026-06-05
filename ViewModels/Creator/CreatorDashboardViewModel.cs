namespace AssetStore.ViewModels.Creator;

public class CreatorDashboardViewModel
{
    public int AssetCount { get; init; }

    public int TransactionCount { get; init; }

    public IReadOnlyList<CreatorAssetItemViewModel> Assets { get; init; } = [];
}

public class CreatorAssetItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }

    public string PriceDisplay => Price == 0 ? "Darmowy" : $"{Price:F2} PLN";
}
