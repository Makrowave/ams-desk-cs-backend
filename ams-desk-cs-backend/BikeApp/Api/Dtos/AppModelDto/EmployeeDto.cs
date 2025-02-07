using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

public partial class EmployeeDto
{
    [Required]
    public short EmployeeId { get; set; }
    [Required]
    [RegularExpression(Regexes.EmployeeName, ErrorMessage = "Niepoprawna nazwa pracownika")]
    public string EmployeeName { get; set; } = null!;
}
