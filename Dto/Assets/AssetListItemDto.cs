namespace AssetStore.Dto.Assets;

public class AssetListItemDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string CreatorName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }
}

