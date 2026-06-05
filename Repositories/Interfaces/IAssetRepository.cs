using AssetStore.Models;
using AssetStore.Models.Common;

namespace AssetStore.Repositories.Interfaces;

public interface IAssetRepository
{
    Task<PagedResult<Asset>> GetPagedAsync(
        int page,
        int pageSize,
        int? categoryId = null,
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default);

    Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Asset?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> GetByCreatorAsync(string creatorId, CancellationToken cancellationToken = default);

    Task<string?> GetCreatorIdAsync(int assetId, CancellationToken cancellationToken = default);

    Task<Asset> CreateAsync(Asset asset, CancellationToken cancellationToken = default);

    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}

