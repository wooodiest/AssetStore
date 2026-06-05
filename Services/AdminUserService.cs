using AssetStore.Dto.Admin;
using AssetStore.Models;
using AssetStore.Models.Common;
using AssetStore.Models.Constants;
using AssetStore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Services;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<AdminUserListItemDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        var result = new List<AdminUserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isUserOnly = roles.Contains(AppRoles.User)
                && !roles.Contains(AppRoles.Creator)
                && !roles.Contains(AppRoles.Administrator);

            result.Add(new AdminUserListItemDto
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? string.Empty,
                Roles = string.Join(", ", roles),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CanPromoteToCreator = isUserOnly && user.IsActive
            });
        }

        return result;
    }

    public async Task<ServiceResult> PromoteToCreatorAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult.Fail("User not found.", ServiceErrorCode.NotFound);
        }

        if (!user.IsActive)
        {
            return ServiceResult.Fail("Cannot promote a blocked user.", ServiceErrorCode.BadRequest);
        }

        if (await _userManager.IsInRoleAsync(user, AppRoles.Creator))
        {
            return ServiceResult.Fail("User already has Creator role.", ServiceErrorCode.Conflict);
        }

        if (await _userManager.IsInRoleAsync(user, AppRoles.Administrator))
        {
            return ServiceResult.Fail("Administrator does not require promotion.", ServiceErrorCode.BadRequest);
        }

        var result = await _userManager.AddToRoleAsync(user, AppRoles.Creator);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(string.Join(" ", result.Errors.Select(e => e.Description)), ServiceErrorCode.BadRequest);
        }

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetUserActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult.Fail("User not found.", ServiceErrorCode.NotFound);
        }

        if (await _userManager.IsInRoleAsync(user, AppRoles.Administrator))
        {
            return ServiceResult.Fail("Cannot block an administrator.", ServiceErrorCode.Forbidden);
        }

        user.IsActive = isActive;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(string.Join(" ", result.Errors.Select(e => e.Description)), ServiceErrorCode.BadRequest);
        }

        return ServiceResult.Ok();
    }
}

