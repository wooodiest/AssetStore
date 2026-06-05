using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

public class AssetsController : Controller
{
    private readonly IAssetAuthorizationService _assetAuthorizationService;

    public AssetsController(IAssetAuthorizationService assetAuthorizationService)
    {
        _assetAuthorizationService = assetAuthorizationService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Purchase(int id)
    {
        return View();
    }

    [Authorize]
    public IActionResult Download(int id)
    {
        return View();
    }

    [Authorize(Roles = AppRoles.Creator)]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    public async Task<IActionResult> Edit(int id)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return Forbid();
        }

        return View();
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _assetAuthorizationService.CanModifyAssetAsync(id))
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Index));
    }
}
