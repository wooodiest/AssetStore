using AssetStore.Dto.Admin;
using AssetStore.Models;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IReviewRepository _reviewRepository;

    public AdminDashboardService(
        UserManager<ApplicationUser> userManager,
        ICategoryRepository categoryRepository,
        IAssetRepository assetRepository,
        IReviewRepository reviewRepository)
    {
        _userManager = userManager;
        _categoryRepository = categoryRepository;
        _assetRepository = assetRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        return new AdminDashboardDto
        {
            UserCount = await _userManager.Users.CountAsync(cancellationToken),
            CategoryCount = categories.Count,
            AssetCount = await _assetRepository.CountActiveAsync(cancellationToken),
            ReviewCount = await _reviewRepository.CountAsync(cancellationToken)
        };
    }
}
