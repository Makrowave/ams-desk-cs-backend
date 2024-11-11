using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.LoginApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly string _role = "Admin";
        private readonly string _cookieName = "admin_token";

        public AdminAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        //[Authorize(Policy = "AccessToken")]
        [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var result = await _authService.Login(user, _role);
            if (result.Status == ServiceStatus.Ok)
            {
                Response.Cookies.Append(_cookieName, result.Data!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(_authService.GetRefreshTokenLenght())
                });
                return Ok();
            }
            return Unauthorized(result.Message);
        }

        [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            string token = Request.Cookies[_cookieName];
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
            return BadRequest();

        }

        private void LogoutCookie()
        {
            Response.Cookies.Append(_cookieName, "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(-1)
            });
        }
    }
}
