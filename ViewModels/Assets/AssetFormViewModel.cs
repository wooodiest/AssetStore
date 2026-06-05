using System.ComponentModel.DataAnnotations;

namespace AssetStore.ViewModels.Assets;

public class AssetFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [StringLength(200)]
    [Display(Name = "Tytuł")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    [StringLength(4000)]
    [Display(Name = "Opis")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999.99)]
    [Display(Name = "Cena (PLN, 0 = darmowy)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Kategoria jest wymagana.")]
    [Display(Name = "Kategoria")]
    public int CategoryId { get; set; }

    [Display(Name = "Plik")]
    public IFormFile? File { get; set; }

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; init; } = [];
}

public class CategoryOptionViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}
