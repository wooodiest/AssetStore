namespace AssetStore.ViewModels.Admin;

public class CategoryListViewModel
{
    public IReadOnlyList<CategoryItemViewModel> Categories { get; init; } = [];
}

public class CategoryItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}

