using AssetStore.Mappings;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize(Roles = AppRoles.Creator)]
public class CreatorController : Controller
{
    private readonly ICreatorDashboardService _creatorDashboardService;
    private readonly ICurrentUserService _currentUserService;

    public CreatorController(
        ICreatorDashboardService creatorDashboardService,
        ICurrentUserService currentUserService)
    {
        _creatorDashboardService = creatorDashboardService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var dashboard = await _creatorDashboardService.GetDashboardAsync(userId, cancellationToken);
        return View(AssetMappings.ToViewModel(dashboard));
    }
}
