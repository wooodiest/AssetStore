namespace AssetStore.Models;

public class Review
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    public Asset Asset { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}

