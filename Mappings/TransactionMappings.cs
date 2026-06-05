using AssetStore.Dto.Transactions;
using AssetStore.ViewModels.Transactions;

namespace AssetStore.Mappings;

public static class TransactionMappings
{
    public static TransactionHistoryViewModel ToViewModel(IReadOnlyList<TransactionListItemDto> items) => new()
    {
        Items = items.Select(i => new TransactionItemViewModel
        {
            Id = i.Id,
            AssetId = i.AssetId,
            AssetTitle = i.AssetTitle,
            CategoryName = i.CategoryName,
            Amount = i.Amount,
            PurchaseDate = i.PurchaseDate
        }).ToList()
    };
}
