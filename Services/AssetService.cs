using AssetStore.Dto.Assets;
using AssetStore.Dto.Categories;
using AssetStore.Mappings;
using AssetStore.Models;
using AssetStore.Models.Common;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IAssetAuthorizationService _assetAuthorizationService;
    private readonly IFileStorageService _fileStorageService;

    public AssetService(
        IAssetRepository assetRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IReviewRepository reviewRepository,
        IAssetAuthorizationService assetAuthorizationService,
        IFileStorageService fileStorageService)
    {
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _reviewRepository = reviewRepository;
        _assetAuthorizationService = assetAuthorizationService;
        _fileStorageService = fileStorageService;
    }

    public async Task<AssetCatalogResultDto> GetCatalogAsync(
        int page,
        int pageSize,
        int? categoryId,
        decimal? maxPrice,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var pagedAssets = await _assetRepository.GetPagedAsync(page, pageSize, categoryId, maxPrice, cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        return new AssetCatalogResultDto
        {
            Assets = new PagedResult<AssetListItemDto>
            {
                Items = pagedAssets.Items.Select(AssetMappings.ToListItemDto).ToList(),
                TotalCount = pagedAssets.TotalCount,
                Page = pagedAssets.Page,
                PageSize = pagedAssets.PageSize
            },
            Categories = categories.Select(AssetMappings.ToCategoryOptionDto).ToList(),
            SelectedCategoryId = categoryId,
            SelectedMaxPrice = maxPrice
        };
    }

    public async Task<AssetDetailsDto?> GetDetailsAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return null;
        }

        var isAuthenticated = !string.IsNullOrEmpty(userId);
        var isOwner = isAuthenticated && asset.CreatorId == userId;
        var hasPurchased = isAuthenticated
            && (isOwner || await _transactionRepository.HasPurchasedAsync(userId!, id, cancellationToken));

        var reviews = asset.Reviews.Select(AssetMappings.ToReviewItemDto).ToList();
        var averageRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;

        var hasReviewed = isAuthenticated
            && await _reviewRepository.ExistsAsync(id, userId!, cancellationToken);

        var hasAcquiredForReview = isAuthenticated
            && !isOwner
            && await _transactionRepository.HasPurchasedAsync(userId!, id, cancellationToken);

        return new AssetDetailsDto
        {
            Id = asset.Id,
            Title = asset.Title,
            Description = asset.Description,
            Price = asset.Price,
            CategoryName = asset.Category?.Name ?? string.Empty,
            CreatorName = asset.Creator?.Email ?? asset.Creator?.UserName ?? "Unknown",
            UploadDate = asset.UploadDate,
            Reviews = reviews,
            AverageRating = averageRating,
            ShowLoginPrompt = !isAuthenticated,
            IsOwner = isOwner,
            HasPurchased = hasPurchased,
            CanPurchase = isAuthenticated && !isOwner && !hasPurchased,
            CanDownload = isAuthenticated && (isOwner || hasPurchased),
            HasReviewed = hasReviewed,
            CanReview = hasAcquiredForReview && !hasReviewed
        };
    }

    public async Task<IReadOnlyList<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(AssetMappings.ToCategoryOptionDto).ToList();
    }

    public async Task<ServiceResult<int>> CreateAssetAsync(
        CreateAssetDto dto,
        string creatorId,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category is null)
        {
            return ServiceResult<int>.Fail("Selected category does not exist.", ServiceErrorCode.BadRequest);
        }

        if (dto.File is null || dto.File.Length == 0)
        {
            return ServiceResult<int>.Fail("Asset file is required.", ServiceErrorCode.BadRequest);
        }

        var fileResult = await _fileStorageService.SaveFileAsync(dto.File, cancellationToken);
        if (!fileResult.Success || fileResult.Data is null)
        {
            return ServiceResult<int>.Fail(
                fileResult.ErrorMessage ?? "Failed to save file.",
                fileResult.ErrorCode);
        }

        var asset = new Asset
        {
            CreatorId = creatorId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            FileUrl = fileResult.Data,
            UploadDate = DateTime.UtcNow,
            IsDeleted = false
        };

        await _assetRepository.CreateAsync(asset, cancellationToken);
        return ServiceResult<int>.Ok(asset.Id);
    }

    public async Task<ServiceResult<UpdateAssetDto>> GetForUpdateAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return ServiceResult<UpdateAssetDto>.Fail("You are not allowed to edit this asset.", ServiceErrorCode.Forbidden);
        }

        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<UpdateAssetDto>.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        return ServiceResult<UpdateAssetDto>.Ok(new UpdateAssetDto
        {
            Title = asset.Title,
            Description = asset.Description,
            Price = asset.Price,
            CategoryId = asset.CategoryId
        });
    }

    public async Task<ServiceResult> UpdateAssetAsync(
        int id,
        UpdateAssetDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return ServiceResult.Fail("You are not allowed to edit this asset.", ServiceErrorCode.Forbidden);
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
        if (category is null)
        {
            return ServiceResult.Fail("Selected category does not exist.", ServiceErrorCode.BadRequest);
        }

        var asset = await _assetRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (asset is null)
        {
            return ServiceResult.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        asset.Title = dto.Title.Trim();
        asset.Description = dto.Description.Trim();
        asset.Price = dto.Price;
        asset.CategoryId = dto.CategoryId;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAssetAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return ServiceResult.Fail("You are not allowed to delete this asset.", ServiceErrorCode.Forbidden);
        }

        var deleted = await _assetRepository.SoftDeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return ServiceResult.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        return ServiceResult.Ok();
    }
}
