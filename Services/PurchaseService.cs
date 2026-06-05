using AssetStore.Dto.Purchase;
using AssetStore.Models;
using AssetStore.Models.Common;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IAssetRepository _assetRepository;
    private readonly ITransactionRepository _transactionRepository;

    public PurchaseService(
        IAssetRepository assetRepository,
        ITransactionRepository transactionRepository)
    {
        _assetRepository = assetRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<ServiceResult<PurchaseResultDto>> PurchaseAssetAsync(
        int assetId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<PurchaseResultDto>.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        if (asset.CreatorId == userId)
        {
            return ServiceResult<PurchaseResultDto>.Ok(new PurchaseResultDto
            {
                CanDownload = true,
                Message = "You own this asset as its creator.",
                AlreadyOwned = true
            });
        }

        if (await _transactionRepository.HasPurchasedAsync(userId, assetId, cancellationToken))
        {
            return ServiceResult<PurchaseResultDto>.Ok(new PurchaseResultDto
            {
                CanDownload = true,
                Message = "You already own this asset.",
                AlreadyOwned = true
            });
        }

        var transaction = new Transaction
        {
            AssetId = assetId,
            UserId = userId,
            Amount = asset.Price,
            PurchaseDate = DateTime.UtcNow
        };

        await _transactionRepository.CreateAsync(transaction, cancellationToken);

        var message = asset.Price == 0
            ? "Free asset acquired successfully."
            : "Purchase completed successfully.";

        return ServiceResult<PurchaseResultDto>.Ok(new PurchaseResultDto
        {
            CanDownload = true,
            Message = message,
            AlreadyOwned = false
        });
    }

    public async Task<ServiceResult<DownloadAuthorizationDto>> GetDownloadAuthorizationAsync(
        int assetId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<DownloadAuthorizationDto>.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        if (asset.CreatorId != userId
            && !await _transactionRepository.HasPurchasedAsync(userId, assetId, cancellationToken))
        {
            return ServiceResult<DownloadAuthorizationDto>.Fail(
                "You must acquire this asset before downloading.",
                ServiceErrorCode.Forbidden);
        }

        return ServiceResult<DownloadAuthorizationDto>.Ok(new DownloadAuthorizationDto
        {
            FileUrl = asset.FileUrl,
            FileName = Path.GetFileName(asset.FileUrl)
        });
    }
}
