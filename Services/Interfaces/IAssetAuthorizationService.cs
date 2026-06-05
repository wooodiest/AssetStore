namespace AssetStore.Services.Interfaces;

public interface IAssetAuthorizationService
{
    Task<bool> CanModifyAssetAsync(int assetId);

    bool CanModifyAsset(string? currentUserId, string assetCreatorId, bool isAdministrator);
}

