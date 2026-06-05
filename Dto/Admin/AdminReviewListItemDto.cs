namespace AssetStore.Dto.Admin;

public class AdminReviewListItemDto
{
    public int Id { get; init; }

    public int AssetId { get; init; }

    public string AssetTitle { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }
}
