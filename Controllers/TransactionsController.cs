using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
