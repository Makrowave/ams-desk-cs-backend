using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Login.Dto;

public class UserDto
{

    [Required]
    public short UserId { get; set; }
    [Required]
    [RegularExpression(Regexes.EmployeeName, ErrorMessage = "Niepoprawna nazwa użytkownika")]
    public string Username { get; set; } = null!;
    [RegularExpression(Regexes.Password, ErrorMessage = "Niepoprawne hasło")]
    public string? Password { get; set; }
    public short? EmployeeId { get; set; }
}