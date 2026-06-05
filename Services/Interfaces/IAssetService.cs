using AssetStore.Dto.Assets;
using AssetStore.Dto.Categories;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface IAssetService
{
    Task<IReadOnlyList<AssetListItemDto>> GetLatestAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<AssetCatalogResultDto> GetCatalogAsync(
        int page,
        int pageSize,
        int? categoryId,
        decimal? maxPrice,
        CancellationToken cancellationToken = default);

    Task<AssetDetailsDto?> GetDetailsAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<int>> CreateAssetAsync(
        CreateAssetDto dto,
        string creatorId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<UpdateAssetDto>> GetForUpdateAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateAssetAsync(
        int id,
        UpdateAssetDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> DeleteAssetAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);
}
