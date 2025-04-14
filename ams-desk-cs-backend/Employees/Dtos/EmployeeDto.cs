using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Employees.Dtos;

public partial class EmployeeDto
{
    [Required]
    public short EmployeeId { get; set; }
    [Required]
    [RegularExpression(Regexes.EmployeeName, ErrorMessage = "Niepoprawna nazwa pracownika")]
    public string EmployeeName { get; set; } = null!;
}
