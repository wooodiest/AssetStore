using AssetStore.Models;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AssetStore.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string?> GetUserIdAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal is null)
        {
            return Task.FromResult<string?>(null);
        }

        var userId = _userManager.GetUserId(principal);
        return Task.FromResult(userId);
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        var isAuthenticated = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        return Task.FromResult(isAuthenticated);
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal is null)
        {
            return null;
        }

        return await _userManager.GetUserAsync(principal);
    }
}

