using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.LoginApp.Api.Controllers
{
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
        public async Task<IActionResult> Login(UserDto user)
        {
            var result = await _authService.Login(user);
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
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            string? token = Request.Cookies[_cookieName];
            if (token == null)
            {
                return Unauthorized("User not logged in");
            }
            return Ok(_authService.Refresh(token));
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
        public async Task<IActionResult> ChangePassword(UserDto userDto)
        {
            var result = await _authService.ChangePassword(userDto);
            if (result.Status == ServiceStatus.Ok)
            {
                LogoutCookie();
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
}
