namespace AssetStore.Dto.Creator;

public class CreatorDashboardDto
{
    public int AssetCount { get; init; }

    public int TransactionCount { get; init; }

    public IReadOnlyList<CreatorAssetItemDto> Assets { get; init; } = [];
}
