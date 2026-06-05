using AssetStore.Dto.Assets;
using AssetStore.Extensions;
using AssetStore.Mappings;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using AssetStore.ViewModels.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

public class AssetsController : Controller
{
    private const int DefaultPageSize = 9;

    private readonly IAssetService _assetService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPurchaseService _purchaseService;
    private readonly IFileStorageService _fileStorageService;

    public AssetsController(
        IAssetService assetService,
        ICurrentUserService currentUserService,
        IPurchaseService purchaseService,
        IFileStorageService fileStorageService)
    {
        _assetService = assetService;
        _currentUserService = currentUserService;
        _purchaseService = purchaseService;
        _fileStorageService = fileStorageService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int? categoryId = null, decimal? maxPrice = null, CancellationToken cancellationToken = default)
    {
        var catalog = await _assetService.GetCatalogAsync(page, DefaultPageSize, categoryId, maxPrice, cancellationToken);
        return View(AssetMappings.ToViewModel(catalog));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        var details = await _assetService.GetDetailsAsync(id, userId, cancellationToken);
        if (details is null)
        {
            return NotFound();
        }

        return View(AssetMappings.ToViewModel(details));
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
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var categories = await _assetService.GetCategoryOptionsAsync(cancellationToken);
        return View(AssetMappings.ToCreateViewModel(categories));
    }

    [Authorize(Roles = AppRoles.Creator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetFormViewModel model, CancellationToken cancellationToken)
    {
        var categories = await _assetService.GetCategoryOptionsAsync(cancellationToken);
        if (model.File is null || model.File.Length == 0)
        {
            ModelState.AddModelError(nameof(model.File), "Plik jest wymagany.");
        }

        if (!ModelState.IsValid)
        {
            return View(RebuildFormViewModel(model, categories));
        }

        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var dto = new CreateAssetDto
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            File = model.File
        };

        var result = await _assetService.CreateAssetAsync(dto, userId, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(RebuildFormViewModel(model, categories));
        }

        TempData["Success"] = "Asset został dodany.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _assetService.GetForUpdateAsync(id, userId, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            return this.ToActionResult(result, _ => RedirectToAction(nameof(Index)));
        }

        var categories = await _assetService.GetCategoryOptionsAsync(cancellationToken);
        return View(AssetMappings.ToEditViewModel(result.Data, id, categories));
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssetFormViewModel model, CancellationToken cancellationToken)
    {
        var categories = await _assetService.GetCategoryOptionsAsync(cancellationToken);
        if (!ModelState.IsValid)
        {
            model = RebuildFormViewModel(model, categories, id);
            return View(model);
        }

        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var dto = new UpdateAssetDto
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId
        };

        var result = await _assetService.UpdateAssetAsync(id, dto, userId, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(RebuildFormViewModel(model, categories, id));
        }

        TempData["Success"] = "Asset został zaktualizowany.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = AppRoles.CreatorOrAdministrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var result = await _assetService.DeleteAssetAsync(id, userId, cancellationToken);
        return this.ToActionResult(result, () =>
        {
            TempData["Success"] = "Asset został usunięty.";
            return RedirectToAction("Index", "Creator");
        });
    }

    private static AssetFormViewModel RebuildFormViewModel(
        AssetFormViewModel model,
        IReadOnlyList<Dto.Categories.CategoryOptionDto> categories,
        int? id = null)
    {
        return new AssetFormViewModel
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            Categories = categories.Select(c => new CategoryOptionViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList()
        };
    }
}
