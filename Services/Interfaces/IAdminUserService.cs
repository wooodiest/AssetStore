using AssetStore.Dto.Admin;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface IAdminUserService
{
    Task<IReadOnlyList<AdminUserListItemDto>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult> PromoteToCreatorAsync(string userId, CancellationToken cancellationToken = default);

    Task<ServiceResult> SetUserActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
}
