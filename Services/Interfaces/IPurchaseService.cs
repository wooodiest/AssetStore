using AssetStore.Dto.Purchase;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface IPurchaseService
{
    Task<ServiceResult<PurchaseResultDto>> PurchaseAssetAsync(
        int assetId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<DownloadAuthorizationDto>> GetDownloadAuthorizationAsync(
        int assetId,
        string userId,
        CancellationToken cancellationToken = default);
}
