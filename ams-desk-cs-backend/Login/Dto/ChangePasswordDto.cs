using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Login.Dto;

public class ChangePasswordDto
{
    //Should be UserId, but don't want to change it for now - UserId works fine, it's unique
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Password, ErrorMessage = "Niepoprawne hasło")]
    public string Password { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Password, ErrorMessage = "Niepoprawne hasło")]
    public string NewPassword { get; set; } = null!;
}