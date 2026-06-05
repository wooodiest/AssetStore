using AssetStore.Dto.Reviews;
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
            TempData["Error"] = "Nieprawidłowe dane recenzji.";
            return RedirectToAction("Details", "Assets", new { id = dto.AssetId });
        }

        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _reviewService.CreateReviewAsync(dto, userId, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction("Details", "Assets", new { id = dto.AssetId });
        }

        TempData["Success"] = "Recenzja została dodana.";
        return RedirectToAction("Details", "Assets", new { id = dto.AssetId });
    }

    [Authorize(Roles = AppRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int assetId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.DeleteReviewAsync(id, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
        }
        else
        {
            TempData["Success"] = "Recenzja została usunięta.";
        }

        return RedirectToAction("Details", "Assets", new { id = assetId });
    }
}
