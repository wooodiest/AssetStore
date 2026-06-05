using AssetStore.Dto.Creator;
using AssetStore.Mappings;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class CreatorDashboardService : ICreatorDashboardService
{
    private readonly IAssetRepository _assetRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CreatorDashboardService(
        IAssetRepository assetRepository,
        ITransactionRepository transactionRepository)
    {
        _assetRepository = assetRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<CreatorDashboardDto> GetDashboardAsync(string creatorId, CancellationToken cancellationToken = default)
    {
        var assets = await _assetRepository.GetByCreatorAsync(creatorId, cancellationToken);
        var transactionCount = await _transactionRepository.CountByCreatorAsync(creatorId, cancellationToken);

        return new CreatorDashboardDto
        {
            AssetCount = assets.Count,
            TransactionCount = transactionCount,
            Assets = assets.Select(AssetMappings.ToCreatorAssetItemDto).ToList()
        };
    }
}
