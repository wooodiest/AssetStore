using System.ComponentModel.DataAnnotations;

namespace AssetStore.Dto.Categories;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}
