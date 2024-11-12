using ams_desk_cs_backend.LoginApp.Api.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.LoginApp.Application.Interfaces
{
    public interface IAdminAuthService : IAuthService
    {
        public Task<ServiceResult> ChangeUserPassword(UserDto user);
    }
}
