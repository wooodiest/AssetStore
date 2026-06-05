namespace AssetStore.Models;

public class Transaction
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public Asset Asset { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
