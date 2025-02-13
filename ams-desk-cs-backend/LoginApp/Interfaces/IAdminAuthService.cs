using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.LoginApp.Interfaces
{
    public interface IAdminAuthService : IAuthService
    {
        public Task<ServiceResult> ChangeUserPassword(ChangePasswordDto user);
    }
}
