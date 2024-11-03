using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.LoginService.Models;

public partial class User
{
    public short UserId { get; set; }

    public required string Username { get; set; }

    public required string Hash { get; set; }
}
