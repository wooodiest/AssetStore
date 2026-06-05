using AssetStore.Dto.Categories;
using AssetStore.Mappings;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using AssetStore.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize(Roles = AppRoles.Administrator)]
public class AdminController : Controller
{
    private readonly IAdminDashboardService _adminDashboardService;
    private readonly ICategoryService _categoryService;
    private readonly IAdminUserService _adminUserService;
    private readonly IReviewService _reviewService;

    public AdminController(
        IAdminDashboardService adminDashboardService,
        ICategoryService categoryService,
        IAdminUserService adminUserService,
        IReviewService reviewService)
    {
        _adminDashboardService = adminDashboardService;
        _categoryService = categoryService;
        _adminUserService = adminUserService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await _adminDashboardService.GetDashboardAsync(cancellationToken);
        return View(AdminMappings.ToViewModel(dashboard));
    }

    public async Task<IActionResult> Categories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return View(AdminMappings.ToViewModel(categories));
    }

    public IActionResult CreateCategory()
    {
        return View(new CategoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CategoryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new CreateCategoryDto
        {
            Name = model.Name,
            Description = model.Description
        };

        var result = await _categoryService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            return View(model);
        }

        TempData["Success"] = "Kategoria została utworzona.";
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> EditCategory(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        return View(AdminMappings.ToFormViewModel(category));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, CategoryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var dto = new UpdateCategoryDto
        {
            Name = model.Name,
            Description = model.Description
        };

        var result = await _categoryService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            model.Id = id;
            return View(model);
        }

        TempData["Success"] = "Kategoria została zaktualizowana.";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(id, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? "Kategoria została usunięta."
            : result.ErrorMessage;

        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var users = await _adminUserService.GetUsersAsync(cancellationToken);
        return View(AdminMappings.ToViewModel(users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PromoteToCreator(string userId, CancellationToken cancellationToken)
    {
        var result = await _adminUserService.PromoteToCreatorAsync(userId, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? "Użytkownik otrzymał rolę Creator."
            : result.ErrorMessage;

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserActive(string userId, bool isActive, CancellationToken cancellationToken)
    {
        var result = await _adminUserService.SetUserActiveAsync(userId, isActive, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? (isActive ? "Użytkownik został odblokowany." : "Użytkownik został zablokowany.")
            : result.ErrorMessage;

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Reviews(CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllForAdminAsync(cancellationToken);
        return View(AdminMappings.ToViewModel(reviews));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int id, CancellationToken cancellationToken)
    {
        var result = await _reviewService.DeleteReviewAsync(id, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Success
            ? "Recenzja została usunięta."
            : result.ErrorMessage;

        return RedirectToAction(nameof(Reviews));
    }
}
