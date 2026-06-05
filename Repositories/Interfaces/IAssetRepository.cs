namespace AssetStore.Repositories.Interfaces;

public interface IAssetRepository
{
    Task<string?> GetCreatorIdAsync(int assetId, CancellationToken cancellationToken = default);
}
