using System.IdentityModel.Tokens.Jwt;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;

namespace ams_desk_cs_backend.Login.Authorization;

public class VersionAuthorizationHandler : AuthorizationHandler<VersionRequirement>
{
    private readonly BikesDbContext _userCredContext;
    public VersionAuthorizationHandler(BikesDbContext userCredContext)
    {
        _userCredContext = userCredContext;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VersionRequirement requirement)
    {
        var nameClaim = context.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var versionClaim = context.User.FindFirst(JwtApplicationClaimNames.Version)?.Value;
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
        if (!int.TryParse(versionClaim, out int tokenVersion) ||
            !short.TryParse(subClaim, out short userId))
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