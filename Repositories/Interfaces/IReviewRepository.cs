using AssetStore.Models;

namespace AssetStore.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Review>> GetByAssetAsync(int assetId, CancellationToken cancellationToken = default);

    Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int assetId, string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
