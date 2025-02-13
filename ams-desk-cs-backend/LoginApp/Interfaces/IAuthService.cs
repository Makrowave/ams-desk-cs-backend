using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.LoginApp.Interfaces
{
    public interface IAuthService
    {
        public Task<ServiceResult<string>> Login(LoginDto user, bool mobile);
        public ServiceResult<string> Refresh(string token);
        public Task<ServiceResult> ChangePassword(ChangePasswordDto user);
        public int GetAccessTokenLenght();
        public int GetRefreshTokenLenght();
    }
}
