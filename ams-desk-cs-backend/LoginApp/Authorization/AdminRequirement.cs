﻿using Microsoft.AspNetCore.Authorization;

namespace ams_desk_cs_backend.LoginApp.Authorization
{
    public class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement() { }
    }
}
