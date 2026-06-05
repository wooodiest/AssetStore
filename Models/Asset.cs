namespace AssetStore.Models;

public class Asset
{
    public int Id { get; set; }

    public string CreatorId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public string FileUrl { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public string ThumbnailUrl { get; set; } = string.Empty;

    public ApplicationUser Creator { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];
}

