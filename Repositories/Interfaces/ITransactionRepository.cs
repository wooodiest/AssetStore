using AssetStore.Models;

namespace AssetStore.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> HasPurchasedAsync(string userId, int assetId, CancellationToken cancellationToken = default);
}
