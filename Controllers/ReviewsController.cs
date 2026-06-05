using AssetStore.Dto.Reviews;
using AssetStore.Extensions;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUserService;

    public ReviewsController(IReviewService reviewService, ICurrentUserService currentUserService)
    {
        _reviewService = reviewService;
        _currentUserService = currentUserService;
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid review data.";
            return RedirectToAction("Details", "Assets", new { id = dto.AssetId });
        }

        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _reviewService.CreateReviewAsync(dto, userId, cancellationToken);
        return this.ToActionResult(result, _ =>
        {
            TempData["Success"] = "Review submitted successfully.";
            return RedirectToAction("Details", "Assets", new { id = dto.AssetId });
        });
    }

    [Authorize(Roles = AppRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int assetId)
    {
        return RedirectToAction("Details", "Assets", new { id = assetId });
    }
}
