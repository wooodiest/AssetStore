using AssetStore.Dto.Categories;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ServiceResult<int>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

