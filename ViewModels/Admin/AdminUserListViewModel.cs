namespace AssetStore.ViewModels.Admin;

public class AdminUserListViewModel
{
    public IReadOnlyList<AdminUserItemViewModel> Users { get; init; } = [];
}

public class AdminUserItemViewModel
{
    public string Id { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Roles { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool CanPromoteToCreator { get; init; }
}
