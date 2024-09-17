using ams_desk_cs_backend.LoginService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Isopoh.Cryptography.Argon2;


using ams_desk_cs_backend.LoginService.Models;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.LoginService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserCredContext _context;

        public AuthController(UserCredContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var hash = Argon2.Hash(user.Password);
            Console.WriteLine(hash);

            if (UserExists(user.Username) && Argon2.Verify((await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username)).Hash, user.Password))
            {
                return Ok();
                //return JWT
            }


            return Unauthorized(hash);
        }

        private bool UserExists(string username)
        {
            return _context.Users.Any(x => x.Username == username);
        }
    }
}
