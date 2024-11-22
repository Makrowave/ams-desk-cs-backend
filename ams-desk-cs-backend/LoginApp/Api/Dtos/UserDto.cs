namespace ams_desk_cs_backend.LoginApp.Api.Dtos
{
    public class UserDto
    {

        public short? UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public short? EmployeeId { get; set; }
    }
}
