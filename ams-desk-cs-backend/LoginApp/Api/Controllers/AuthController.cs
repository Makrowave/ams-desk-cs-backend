using Microsoft.AspNetCore.Mvc;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Composition;
using ams_desk_cs_backend.LoginApp.Infrastructure.Data;
using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using NuGet.Common;

namespace ams_desk_cs_backend.LoginApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var result = await _authService.Login(user);
            if(result.Status == ServiceStatus.Ok)
            {
                Response.Cookies.Append("refresh_token", result.Data!, new CookieOptions
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
        public async Task<IActionResult> Refresh()
        {
            //Should be valid JWT anyways since it goes through Authorize so no check
            string token = Request.Cookies["refresh_token"]!;
            var result = await _authService.ValidateToken(token);
            if (result.Status == ServiceStatus.Ok)
            {
                return Ok(_authService.Refresh(
                    result.Data![JwtRegisteredClaimNames.Name],
                    result.Data![ClaimTypes.Version],
                    result.Data![JwtRegisteredClaimNames.Sub]
                    ));
            }
            LogoutCookie();
            return Unauthorized();
        }

        [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            LogoutCookie();
            return Ok();
        }
        [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserDto userDto)
        {
            string token = Request.Cookies["refresh_token"]!;
            var result = await _authService.ValidateToken(token);
            if (result.Status == ServiceStatus.Ok)
            {
                var hash = Argon2.Hash(userDto.NewPassword);
                var changeResult = _authService.ChangePassword(userDto); //? why not async????
                LogoutCookie();
                return Ok();
            }
            return BadRequest();
        }

        private void LogoutCookie()
        {
            Response.Cookies.Append("refresh_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(-1)
            });
        }
    }
}
