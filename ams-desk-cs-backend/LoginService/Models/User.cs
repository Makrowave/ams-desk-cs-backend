using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.LoginService.Models;

public partial class User
{
    public short UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Hash { get; set; } = null!;
}
