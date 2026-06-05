namespace AssetStore.Services.Interfaces;

public interface ICurrentUserService
{
    Task<string?> GetUserIdAsync();

    Task<bool> IsInRoleAsync(string role);

    Task<bool> IsAuthenticatedAsync();
}

