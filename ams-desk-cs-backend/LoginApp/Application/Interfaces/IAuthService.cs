using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.LoginApp.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ServiceResult<string>> Login(UserDto user);
        public string Refresh(string name, string version, string id);
        public ServiceResult ChangePassword(UserDto user);
        public Task<ServiceResult<Dictionary<string, string>>> ValidateToken(string token);
        public int GetAccessTokenLenght();
        public int GetRefreshTokenLenght();
    }
}
