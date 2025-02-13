using ams_desk_cs_backend.LoginApp.Data;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace ams_desk_cs_backend.LoginApp.Authorization
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly UserCredContext _userCredContext;
        public AdminAuthorizationHandler(UserCredContext userCredContext)
        {
            _userCredContext = userCredContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminRequirement requirement)
        {
            var roleClaim = context.User.FindFirst(JwtApplicationClaimNames.Role)?.Value;
            var subClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (roleClaim == null || subClaim == null)
            {
                context.Fail();
                return;
            }
            if (roleClaim != "Admin")
            {
                context.Fail();
                return;
            }
            if (!short.TryParse(subClaim, out short userId))
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
            if (user!.IsAdmin == false)
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }
}
