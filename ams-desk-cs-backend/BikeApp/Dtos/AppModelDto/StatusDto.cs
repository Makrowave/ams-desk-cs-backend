using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
public class StatusDto
{
    [Required]
    public short StatusId { get; set; }
    [Required]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawna nazwa statusu")]
    public string StatusName { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Name16, ErrorMessage = "Niepoprawny kolor statusu")]
    public string HexCode { get; set; } = null!;
}
