namespace AssetStore.Dto.Assets;

public class AssetDetailsDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string CreatorName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }

    public IReadOnlyList<ReviewItemDto> Reviews { get; init; } = [];

    public double AverageRating { get; init; }

    public bool ShowLoginPrompt { get; init; }

    public bool CanPurchase { get; init; }

    public bool CanDownload { get; init; }

    public bool HasPurchased { get; init; }

    public bool IsOwner { get; init; }

    public bool CanReview { get; init; }

    public bool HasReviewed { get; init; }
}

