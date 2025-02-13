using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.LoginApp.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResult<IEnumerable<UserDto>>> GetUsers();
        public Task<ServiceResult> PostUser(UserDto user);
        public Task<ServiceResult> ChangeUser(short id, UserDto user);
        public Task<ServiceResult> DeleteUser(short id);
        public Task<ServiceResult> LogOutUser(short id);
        public Task<ServiceResult> LogOutAll();

    }
}
