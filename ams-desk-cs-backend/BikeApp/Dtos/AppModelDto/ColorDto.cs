using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

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
