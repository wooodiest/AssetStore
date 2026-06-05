namespace AssetStore.Dto.Creator;

public class CreatorAssetItemDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }
}

