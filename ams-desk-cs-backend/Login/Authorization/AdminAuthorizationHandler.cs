using System.IdentityModel.Tokens.Jwt;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;

namespace ams_desk_cs_backend.Login.Authorization;

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly BikesDbContext _context;
    public AdminAuthorizationHandler(BikesDbContext context)
    {
        _context = context;
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
        var user = await _context.Users.FindAsync(userId);
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