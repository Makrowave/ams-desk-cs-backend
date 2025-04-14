using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.BikeFilters.Dtos;
public partial class ManufacturerDto
{
    [Required]
    public short ManufacturerId { get; set; }
    [Required]
    [RegularExpression(Regexes.NameAnyCase16, ErrorMessage = "Niepoprawna nazwa producenta")]
    public string ManufacturerName { get; set; } = null!;
}
