using AssetStore.Models.Constants;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class AssetAuthorizationService : IAssetAuthorizationService
{
    private readonly IAssetRepository _assetRepository;
    private readonly ICurrentUserService _currentUserService;

    public AssetAuthorizationService(
        IAssetRepository assetRepository,
        ICurrentUserService currentUserService)
    {
        _assetRepository = assetRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> CanModifyAssetAsync(int assetId)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        var creatorId = await _assetRepository.GetCreatorIdAsync(assetId);
        if (creatorId is null)
        {
            return false;
        }

        var isAdministrator = await _currentUserService.IsInRoleAsync(AppRoles.Administrator);
        return CanModifyAsset(userId, creatorId, isAdministrator);
    }

    public bool CanModifyAsset(string? currentUserId, string assetCreatorId, bool isAdministrator)
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            return false;
        }

        return isAdministrator || currentUserId == assetCreatorId;
    }
}
