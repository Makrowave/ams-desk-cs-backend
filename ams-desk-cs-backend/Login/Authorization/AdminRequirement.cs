﻿using Microsoft.AspNetCore.Authorization;

namespace ams_desk_cs_backend.Login.Authorization;

public class AdminRequirement : IAuthorizationRequirement
{
    public AdminRequirement() { }
}