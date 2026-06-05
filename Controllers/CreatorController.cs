using AssetStore.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize(Roles = AppRoles.Creator)]
public class CreatorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
