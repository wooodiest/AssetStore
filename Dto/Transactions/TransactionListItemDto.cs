namespace AssetStore.Dto.Transactions;

public class TransactionListItemDto
{
    public int Id { get; init; }

    public int AssetId { get; init; }

    public string AssetTitle { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public DateTime PurchaseDate { get; init; }
}
