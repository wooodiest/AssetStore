using AssetStore.Dto.Categories;
using AssetStore.Models;
using AssetStore.Models.Common;
using AssetStore.Repositories.Interfaces;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(ToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : ToDto(category);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim()
        };

        await _categoryRepository.CreateAsync(category, cancellationToken);
        return ServiceResult<int>.Ok(category.Id);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return ServiceResult.Fail("Category not found.", ServiceErrorCode.NotFound);
        }

        category.Name = dto.Name.Trim();
        category.Description = dto.Description.Trim();
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _categoryRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return ServiceResult.Fail(
                "Cannot delete category - either it has assigned assets or it does not exist.",
                ServiceErrorCode.BadRequest);
        }

        return ServiceResult.Ok();
    }

    private static CategoryDto ToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };
}

