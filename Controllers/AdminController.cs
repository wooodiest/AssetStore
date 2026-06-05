using AssetStore.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize(Roles = AppRoles.Administrator)]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Categories()
    {
        return View();
    }

    public IActionResult Users()
    {
        return View();
    }

    public IActionResult Reviews()
    {
        return View();
    }
}
