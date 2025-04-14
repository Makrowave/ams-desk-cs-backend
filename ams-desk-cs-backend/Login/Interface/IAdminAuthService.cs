using ams_desk_cs_backend.Login.Dto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Login.Interface;

public interface IAdminAuthService : IAuthService
{
    public Task<ServiceResult> ChangeUserPassword(ChangePasswordDto user);
}