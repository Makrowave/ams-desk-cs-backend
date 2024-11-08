using ams_desk_cs_backend.LoginApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ams_desk_cs_backend.LoginApp.Application.Authorization
{
    public class VersionAuthorizationHandler : AuthorizationHandler<VersionRequirement>
    {
        private readonly UserCredContext _userCredContext;
        public VersionAuthorizationHandler(UserCredContext userCredContext)
        {
            _userCredContext = userCredContext;
        }
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            VersionRequirement requirement)
        {
            var nameClaim = context.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            var versionClaim = context.User.FindFirst(ClaimTypes.Version)?.Value;
            var subClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string output = "\n";
            foreach (var claim in context.User.Claims)
            {
                output += claim.ToString() + "\n";
            }
            if (nameClaim == null || versionClaim == null || subClaim == null)
            {
                context.Fail();
                return;
            }
            short userId = -1;
            int tokenVersion;
            if (!int.TryParse(versionClaim, out tokenVersion) ||
                !short.TryParse(subClaim, out userId))
            {
                context.Fail();
                return;
            }
            var user = await _userCredContext.Users.FindAsync(userId);
            if (user == null)
            {
                context.Fail();
                return;
            }
            if (user!.TokenVersion != tokenVersion)
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }
}
