using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

public class CategoryDto
{
    [Required(ErrorMessage = "Niepoprawna kategoria")]
    public short CategoryId { get; set; }
    [Required(ErrorMessage = "Brak nazwy kategorii")]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawna nazwa kategorii")]
    public string CategoryName { get; set; } = null!;
}
