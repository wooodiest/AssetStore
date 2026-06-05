using System.ComponentModel.DataAnnotations;

namespace AssetStore.Dto.Categories;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}

