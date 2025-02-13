using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.LoginApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public MobileAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto user)
        {
            var result = await _authService.Login(user, true);
            if (result.Status == ServiceStatus.Ok)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [Authorize(Policy = "MobileRefreshToken", AuthenticationSchemes = "MobileRefreshToken")]
        [HttpGet("Refresh")]
        public IActionResult Refresh()
        {
            var auth = Request.Headers["Authorization"].FirstOrDefault();
            if (auth == null || !auth.StartsWith("Bearer "))
            {
                return Unauthorized("User not logged in");
            }
            var token = auth.Substring("Bearer ".Length).Trim();
            if (token == "")
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
    }
}
