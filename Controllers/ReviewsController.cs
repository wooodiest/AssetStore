using AssetStore.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

public class ReviewsController : Controller
{
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int assetId)
    {
        return RedirectToAction("Details", "Assets", new { id = assetId });
    }

    [Authorize(Roles = AppRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int assetId)
    {
        return RedirectToAction("Details", "Assets", new { id = assetId });
    }
}
