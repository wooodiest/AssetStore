using AssetStore.Dto.Reviews;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface IReviewService
{
    Task<ServiceResult<ReviewResponseDto>> CreateReviewAsync(
        CreateReviewDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> DeleteReviewAsync(
        int reviewId,
        CancellationToken cancellationToken = default);
}
