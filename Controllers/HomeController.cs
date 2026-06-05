using AssetStore.Mappings;
using AssetStore.Models;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AssetStore.Controllers;

public class HomeController : Controller
{
    private const int FeaturedAssetCount = 6;

    private readonly IAssetService _assetService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IAssetService assetService, ILogger<HomeController> logger)
    {
        _assetService = assetService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var latest = await _assetService.GetLatestAsync(FeaturedAssetCount, cancellationToken);
        return View(HomeMappings.ToViewModel(latest));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

