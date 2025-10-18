using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.BikeFilters.Dtos;
public class StatusDto
{
    [Required]
    public short Id { get; set; }
    [Required]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawna nazwa statusu")]
    public string Name { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Color, ErrorMessage = "Niepoprawny kolor statusu")]
    public string Color { get; set; } = null!;
}
