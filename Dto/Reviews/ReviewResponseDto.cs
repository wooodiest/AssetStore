namespace AssetStore.Dto.Reviews;

public class ReviewResponseDto
{
    public int Id { get; init; }

    public int AssetId { get; init; }

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }
}
