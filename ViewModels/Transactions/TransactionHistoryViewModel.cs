namespace AssetStore.ViewModels.Transactions;

public class TransactionHistoryViewModel
{
    public IReadOnlyList<TransactionItemViewModel> Items { get; init; } = [];
}

public class TransactionItemViewModel
{
    public int Id { get; init; }

    public int AssetId { get; init; }

    public string AssetTitle { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public DateTime PurchaseDate { get; init; }

    public string AmountDisplay => Amount == 0 ? "Free" : $"{Amount:F2} PLN";
}

