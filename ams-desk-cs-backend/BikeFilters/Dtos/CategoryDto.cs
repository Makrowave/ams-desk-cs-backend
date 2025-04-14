using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.BikeFilters.Dtos;

public class CategoryDto
{
    [Required(ErrorMessage = "Niepoprawna kategoria")]
    public short CategoryId { get; set; }
    [Required(ErrorMessage = "Brak nazwy kategorii")]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawna nazwa kategorii")]
    public string CategoryName { get; set; } = null!;
}
