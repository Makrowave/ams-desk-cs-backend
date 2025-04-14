using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Login.Dto;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}