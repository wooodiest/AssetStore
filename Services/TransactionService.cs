using AssetStore.Dto.Transactions;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IReadOnlyList<TransactionListItemDto>> GetUserHistoryAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByUserAsync(userId, cancellationToken);

        return transactions.Select(t => new TransactionListItemDto
        {
            Id = t.Id,
            AssetId = t.AssetId,
            AssetTitle = t.Asset?.Title ?? "Unknown asset",
            CategoryName = t.Asset?.Category?.Name ?? string.Empty,
            Amount = t.Amount,
            PurchaseDate = t.PurchaseDate
        }).ToList();
    }
}

