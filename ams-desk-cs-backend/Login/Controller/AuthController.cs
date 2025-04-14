using ams_desk_cs_backend.Login.Dto;
using ams_desk_cs_backend.Login.Interface;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Login.Controller;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly string _cookieName = "refresh_token";

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto user)
    {
        var result = await _authService.Login(user, false);
        if (result.Status == ServiceStatus.Ok)
        {
            Response.Cookies.Append(_cookieName, result.Data!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(_authService.GetRefreshTokenLenght())
            });
            return Ok();
        }
        return BadRequest(result.Message);
    }

    [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
    [HttpGet("Refresh")]
    public IActionResult Refresh()
    {
        string? token = Request.Cookies[_cookieName];
        if (token == null)
        {
            return Unauthorized("User not logged in");
        }
        var result = _authService.Refresh(token);
        if (result.Status == ServiceStatus.Unauthorized)
        {
            return Unauthorized(result.Message);
        }
        return Ok(result.Data);
    }

    [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        LogoutCookie();
        return Ok();
    }
    //Finish this
    [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto userDto)
    {
        var result = await _authService.ChangePassword(userDto);
        if (result.Status == ServiceStatus.Ok)
        {
            return Ok();
        }
        return BadRequest(result.Message);

    }

    private void LogoutCookie()
    {
        Response.Cookies.Append(_cookieName, "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(-1)
        });
    }
}