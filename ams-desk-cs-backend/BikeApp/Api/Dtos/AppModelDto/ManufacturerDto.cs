using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
public partial class ManufacturerDto
{
    [Required]
    public short ManufacturerId { get; set; }
    [Required]
    [RegularExpression(Regexes.NameAnyCase16, ErrorMessage = "Niepoprawna nazwa producenta")]
    public string ManufacturerName { get; set; } = null!;
}
