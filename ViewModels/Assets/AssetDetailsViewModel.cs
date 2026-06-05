namespace AssetStore.ViewModels.Assets;

public class AssetDetailsViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string CreatorName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }

    public double AverageRating { get; init; }

    public IReadOnlyList<ReviewItemViewModel> Reviews { get; init; } = [];

    public bool ShowLoginPrompt { get; init; }

    public bool CanPurchase { get; init; }

    public bool CanDownload { get; init; }

    public bool HasPurchased { get; init; }

    public bool IsOwner { get; init; }

    public bool CanReview { get; init; }

    public bool HasReviewed { get; init; }

    public string PriceDisplay => Price == 0 ? "Darmowy" : $"{Price:F2} PLN";

    public string PurchaseButtonText => Price == 0 ? "Pobierz za darmo" : $"Kup za {Price:F2} PLN";
}

public class ReviewItemViewModel
{
    public int Id { get; init; }

    public string UserName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }
}
