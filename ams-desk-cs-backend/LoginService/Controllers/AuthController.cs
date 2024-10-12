using ams_desk_cs_backend.LoginService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Isopoh.Cryptography.Argon2;


using ams_desk_cs_backend.LoginService.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Composition;

namespace ams_desk_cs_backend.LoginService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserCredContext _context;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;

        public AuthController(UserCredContext context, IConfiguration configuration)
        {
            _context = context;
            _issuer = configuration["Login:JWT:Issuer"];
            _audience = configuration["Login:JWT:Audience"];
            _key = configuration["Login:JWT:Key"];
            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var expiry = 24 * 30;
            var hash = Argon2.Hash(user.Password);
            Console.WriteLine(hash);
            if (UserExists(user.Username) && Argon2.Verify((
                await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username))!.Hash, 
                user.Password))
            {
                var token = GenerateJwtToken(expiry);
                Response.Cookies.Append("refresh_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(expiry)
                });
                return Ok();
            }
            return Unauthorized();
        }

        [Authorize(Policy="RefreshToken", AuthenticationSchemes ="RefreshToken")]
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            return Ok(GenerateJwtToken(1));
        }

        private string GenerateJwtToken(int hours)
        {
            var claims = new Claim[] { };
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                null,
                null,
                expires: DateTime.UtcNow.AddHours(hours),
                signingCredentials
                );
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }

        [Authorize(Policy = "RefreshToken", AuthenticationSchemes = "RefreshToken")]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("refresh_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(-1)
            });
            return Ok();
        }

        private bool UserExists(string username)
        {
            return _context.Users.Any(x => x.Username == username);
        }
    }
}
