using AssetStore.Dto.Admin;
using AssetStore.Dto.Reviews;
using AssetStore.Models;
using AssetStore.Models.Common;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class ReviewService : IReviewService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ITransactionRepository _transactionRepository;

    public ReviewService(
        IAssetRepository assetRepository,
        IReviewRepository reviewRepository,
        ITransactionRepository transactionRepository)
    {
        _assetRepository = assetRepository;
        _reviewRepository = reviewRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<ServiceResult<ReviewResponseDto>> CreateReviewAsync(
        CreateReviewDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (dto.Rating is < 1 or > 5)
        {
            return ServiceResult<ReviewResponseDto>.Fail(
                "Rating must be between 1 and 5.",
                ServiceErrorCode.BadRequest);
        }

        var asset = await _assetRepository.GetByIdAsync(dto.AssetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<ReviewResponseDto>.Fail("Asset not found.", ServiceErrorCode.NotFound);
        }

        if (asset.CreatorId == userId)
        {
            return ServiceResult<ReviewResponseDto>.Fail(
                "You cannot review your own asset.",
                ServiceErrorCode.Forbidden);
        }

        if (!await _transactionRepository.HasPurchasedAsync(userId, dto.AssetId, cancellationToken))
        {
            return ServiceResult<ReviewResponseDto>.Fail(
                "You must purchase this asset before reviewing it.",
                ServiceErrorCode.Forbidden);
        }

        if (await _reviewRepository.ExistsAsync(dto.AssetId, userId, cancellationToken))
        {
            return ServiceResult<ReviewResponseDto>.Fail(
                "You have already reviewed this asset.",
                ServiceErrorCode.Conflict);
        }

        var review = new Review
        {
            AssetId = dto.AssetId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment.Trim(),
            PostedAt = DateTime.UtcNow
        };

        await _reviewRepository.CreateAsync(review, cancellationToken);

        return ServiceResult<ReviewResponseDto>.Ok(new ReviewResponseDto
        {
            Id = review.Id,
            AssetId = review.AssetId,
            Rating = review.Rating,
            Comment = review.Comment,
            PostedAt = review.PostedAt
        });
    }

    public async Task<ServiceResult> DeleteReviewAsync(
        int reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
        {
            return ServiceResult.Fail("Review not found.", ServiceErrorCode.NotFound);
        }

        var deleted = await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
        if (!deleted)
        {
            return ServiceResult.Fail("Failed to delete review.", ServiceErrorCode.BadRequest);
        }

        return ServiceResult.Ok();
    }

    public async Task<IReadOnlyList<AdminReviewListItemDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewRepository.GetAllAsync(cancellationToken);

        return reviews.Select(r => new AdminReviewListItemDto
        {
            Id = r.Id,
            AssetId = r.AssetId,
            AssetTitle = r.Asset?.Title ?? "Unknown asset",
            UserName = r.User?.Email ?? r.User?.UserName ?? "Anonymous",
            Rating = r.Rating,
            Comment = r.Comment,
            PostedAt = r.PostedAt
        }).ToList();
    }
}

