namespace AssetStore.Dto.Assets;

public class ReviewItemDto
{
    public int Id { get; init; }

    public string UserName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }
}
