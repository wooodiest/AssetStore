using AssetStore.Dto.Transactions;

namespace AssetStore.Services.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionListItemDto>> GetUserHistoryAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
