using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ams_desk_cs_backend.Login.Data;
using ams_desk_cs_backend.Login.Dto;
using ams_desk_cs_backend.Login.Interface;
using ams_desk_cs_backend.Shared;
using ams_desk_cs_backend.Shared.Results;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ams_desk_cs_backend.Login.Service;

public class AuthService : IAuthService
{
    private readonly UserCredContext _context;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;
    private readonly string _role = "User";
    private readonly JwtSecurityTokenHandler _jwtHandler;
    private readonly int _accessTokenLength;
    private readonly int _refreshTokenLength;
    public AuthService(UserCredContext context, IConfiguration configuration)
    {
        _context = context;
        _issuer = configuration["Login:JWT:Issuer"] ?? throw new ArgumentNullException(nameof(configuration));
        _audience = configuration["Login:JWT:Audience"] ?? throw new ArgumentNullException(nameof(configuration));
        _key = configuration["Login:JWT:Key"] ?? throw new ArgumentNullException(nameof(configuration));
        _accessTokenLength = int.Parse(configuration["Login:User:AccessTokenLength"]
                                       ?? throw new ArgumentNullException(nameof(configuration)));
        _refreshTokenLength = int.Parse(configuration["Login:User:RefreshTokenLength"]
                                        ?? throw new ArgumentNullException(nameof(configuration)));
        _jwtHandler = new JwtSecurityTokenHandler();
    }

    public async Task<ServiceResult> ChangePassword(ChangePasswordDto userDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
        if (user == null
            || userDto.Username != user.Username
            || !Argon2.Verify(user.Hash, userDto.Password))
            return new ServiceResult(ServiceStatus.BadRequest, "Nie udało się zmienić hasła");
        user.SetPassword(userDto.NewPassword);
        user.TokenVersion++;
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }

    public async Task<ServiceResult<string>> Login(LoginDto userDto, bool mobile)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);


        if (user == null || !Argon2.Verify(
                user.Hash,
                userDto.Password))
            return new ServiceResult<string>(ServiceStatus.BadRequest, "Nieprawidłowe dane logowania", null);
            
        string id = user.EmployeeId.ToString() ?? "DesktopOnly";
        var token = GenerateJwtToken(_refreshTokenLength, user.Username, user.TokenVersion.ToString(), user.UserId, _role, id, mobile);
        return new ServiceResult<string>(ServiceStatus.Ok, string.Empty, token);

    }

    public ServiceResult<string> Refresh(string token)
    {
        var parsedToken = ParseToken(token);
        try
        {
            return new ServiceResult<string>(ServiceStatus.Ok, string.Empty, GenerateJwtToken(_accessTokenLength,
                parsedToken[JwtRegisteredClaimNames.Name],
                parsedToken[JwtApplicationClaimNames.Version],
                int.Parse(parsedToken[JwtRegisteredClaimNames.Sub]),
                parsedToken[JwtApplicationClaimNames.Role],
                parsedToken[JwtApplicationClaimNames.Employee],
                false));
        }
        catch (Exception)
        {
            return new ServiceResult<string>(ServiceStatus.Unauthorized, "Old token version", string.Empty);
        }
    }

    private Dictionary<string, string> ParseToken(string token)
    {
        return _jwtHandler.ReadJwtToken(token).Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
    }

    private string GenerateJwtToken(int minutes, string name, string version, int id, string role, string employeeId, bool mobileRefresh)
    {
        var claims = new [] {
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim(JwtApplicationClaimNames.Version, version),
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtApplicationClaimNames.Role, role),
            new Claim(JwtApplicationClaimNames.Employee, employeeId)
        };
        if (mobileRefresh)
        {
            claims = [.. claims, new Claim(JwtApplicationClaimNames.Mobile, "true")];
        }
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

    public int GetAccessTokenLenght()
    {
        return _accessTokenLength;
    }

    public int GetRefreshTokenLenght()
    {
        return _refreshTokenLength;
    }
}