using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.LoginApp.Infrastructure.Data.Models;

public partial class User
{
    public short UserId { get; set; }

    public required string Username { get; set; }

    public required string Hash { get; set; }
    public required int TokenVersion { get; set; }
    public required bool IsAdmin { get; set; }
    public string? AdminHash { get; set; }
    

}
