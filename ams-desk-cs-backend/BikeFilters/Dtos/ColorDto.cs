using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.BikeFilters.Dtos;

public class ColorDto
{
    [Required]
    public short ColorId { get; set; }
    [Required]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawna nazwa koloru")]
    public string ColorName { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Color, ErrorMessage = "Niepoprawny kolor")]
    public string HexCode { get; set; } = null!;
}
