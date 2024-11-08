using Microsoft.AspNetCore.Authorization;

namespace ams_desk_cs_backend.LoginApp.Application.Authorization
{
    public class VersionRequirement : IAuthorizationRequirement
    {
        public VersionRequirement() { }
    }
}
