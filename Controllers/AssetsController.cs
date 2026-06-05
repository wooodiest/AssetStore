using AssetStore.Extensions;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

public class AssetsController : Controller
{
    private readonly IAssetAuthorizationService _assetAuthorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPurchaseService _purchaseService;
    private readonly IFileStorageService _fileStorageService;

    public AssetsController(
        IAssetAuthorizationService assetAuthorizationService,
        ICurrentUserService currentUserService,
        IPurchaseService purchaseService,
        IFileStorageService fileStorageService)
    {
        _assetAuthorizationService = assetAuthorizationService;
        _currentUserService = currentUserService;
        _purchaseService = purchaseService;
        _fileStorageService = fileStorageService;
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
    public async Task<IActionResult> Purchase(int id, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _purchaseService.PurchaseAssetAsync(id, userId, cancellationToken);
        return this.ToActionResult(result, data =>
        {
            TempData["Success"] = data.Message;
            return RedirectToAction(nameof(Details), new { id });
        });
    }

    [Authorize]
    public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var authorizationResult = await _purchaseService.GetDownloadAuthorizationAsync(id, userId, cancellationToken);
        if (!authorizationResult.Success || authorizationResult.Data is null)
        {
            TempData["Error"] = authorizationResult.ErrorMessage;
            return RedirectToAction(nameof(Details), new { id });
        }

        var fileResult = await _fileStorageService.GetFileAsync(authorizationResult.Data.FileUrl, cancellationToken);
        return this.ToActionResult(fileResult, file =>
            File(file.Stream, file.ContentType, file.FileName));
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
