using AssetStore.Models;
using Microsoft.AspNetCore.Identity;

namespace AssetStore.Middleware;

public class ActiveUserMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is { IsActive: false })
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Identity/Account/Login?disabled=true");
                return;
            }
        }

        await _next(context);
    }
}
