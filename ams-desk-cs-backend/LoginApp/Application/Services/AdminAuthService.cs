using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.LoginApp.Infrastructure.Data;
using ams_desk_cs_backend.LoginApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.Shared;
using ams_desk_cs_backend.Shared.Results;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ams_desk_cs_backend.LoginApp.Application.Services
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly UserCredContext _context;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly string _role = "Admin";
        private readonly JwtSecurityTokenHandler _jwtHandler;
        public readonly int _accessTokenLength;
        public readonly int _refreshTokenLength;
        public AdminAuthService(UserCredContext context, IConfiguration configuration)
        {
            _context = context;
            _issuer = configuration["Login:JWT:Issuer"] ?? throw new ArgumentNullException(nameof(configuration));
            _audience = configuration["Login:JWT:Audience"] ?? throw new ArgumentNullException(nameof(configuration));
            _key = configuration["Login:JWT:Key"] ?? throw new ArgumentNullException(nameof(configuration));
            _accessTokenLength = Int32.Parse(configuration["Login:Admin:AccessTokenLength"]
                ?? throw new ArgumentNullException(nameof(configuration)));
            _refreshTokenLength = Int32.Parse(configuration["Login:Admin:RefreshTokenLength"]
                ?? throw new ArgumentNullException(nameof(configuration)));
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public async Task<ServiceResult> ChangePassword(ChangePasswordDto userDto)
        {
            User? user = null;
            if (userDto.Username != null)
            {
                user = await GetUserAsync(userDto.Username);
            }
            if (user != null
                && userDto.Username == user.Username
                && userDto.Password != null
                && userDto.NewPassword != null
                && user.IsAdmin
                && user.AdminHash != null
                && Argon2.Verify(user.AdminHash, userDto.Password))
            {
                var hash = Argon2.Hash(userDto.NewPassword);
                user.AdminHash = hash;
                user.TokenVersion++;
                await _context.SaveChangesAsync();
                return new ServiceResult(ServiceStatus.Ok, String.Empty);
            }
            return new ServiceResult(ServiceStatus.BadRequest, "Nie udało się zmienić hasła");
        }

        public async Task<ServiceResult<string>> Login(LoginDto userDto, bool mobile)
        {
            var hash = Argon2.Hash(userDto.Password);
            //Fetch user
            User user = (await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username))!;
            //Check password
            if (user != null && user.IsAdmin && user.AdminHash != null && Argon2.Verify(
            user.AdminHash,
            userDto.Password))
            {
                var token = GenerateJwtToken(_refreshTokenLength, user.Username, user.TokenVersion.ToString(), user.UserId, _role);
                return new ServiceResult<string>(ServiceStatus.Ok, String.Empty, token);
            }

            return new ServiceResult<string>(ServiceStatus.BadRequest, "Nieprawidłowe dane logowania", null);
        }

        public ServiceResult<string> Refresh(string token)
        {
            var parsedToken = ParseToken(token);
            try
            {
                return new ServiceResult<string>(ServiceStatus.Ok, string.Empty, GenerateJwtToken(_accessTokenLength,
                    parsedToken[JwtRegisteredClaimNames.Name],
                    parsedToken[JwtApplicationClaimNames.Version],
                    Int32.Parse(parsedToken[JwtRegisteredClaimNames.Sub]),
                    parsedToken[JwtApplicationClaimNames.Role]));
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(ServiceStatus.Unauthorized, "Old token version", string.Empty);
            }
        }
                // Most of these are same as AuthService
        private Dictionary<string, string> ParseToken(string token)
        {
            return _jwtHandler.ReadJwtToken(token).Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
        }

        private string GenerateJwtToken(int minutes, string name, string version, int id, string role)
        {
            var claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Name, name),
                new Claim(JwtApplicationClaimNames.Version, version),
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtApplicationClaimNames.Role, role)
            };
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                null,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials
                );
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }
        private bool UserExists(string username)
        {
            return _context.Users.Any(x => x.Username == username);
        }

        public int GetAccessTokenLenght()
        {
            return _accessTokenLength;
        }

        public int GetRefreshTokenLenght()
        {
            return _refreshTokenLength;
        }

        private async Task<User> GetUserAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public Task<ServiceResult> ChangeUserPassword(ChangePasswordDto user)
        {
            throw new NotImplementedException();
        }
    }
}
