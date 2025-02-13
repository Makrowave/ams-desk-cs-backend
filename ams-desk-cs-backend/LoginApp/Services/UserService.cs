using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.LoginApp.Data;
using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.LoginApp.Data.Models;
using ams_desk_cs_backend.LoginApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.LoginApp.Services
{
    public class UserService : IUserService
    {
        private readonly BikesDbContext _bikesDbContext;
        private readonly UserCredContext _userCredContext;
        public UserService(BikesDbContext bikesDbContext, UserCredContext userCredContext)
        {
            _bikesDbContext = bikesDbContext;
            _userCredContext = userCredContext;
        }

        public async Task<ServiceResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userCredContext.Users.Select(user => new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                EmployeeId = user.EmployeeId,

            }).OrderBy(user => user.UserId).ToListAsync();
            return new ServiceResult<IEnumerable<UserDto>>(ServiceStatus.Ok, string.Empty, users);
        }

        public async Task<ServiceResult> PostUser(UserDto user)
        {
            _userCredContext.Add(new User
            {
                Username = user.Username,
                EmployeeId = user.EmployeeId,
                Hash = Argon2.Hash(user.Password),
                IsAdmin = false,
                TokenVersion = 1
            });
            try
            {
                await _userCredContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Nie można przypisać pracownika do drugiego konta");
            }
            return new ServiceResult(ServiceStatus.Ok, string.Empty);

        }
        public async Task<ServiceResult> ChangeUser(short id, UserDto newUser)
        {
            var oldUser = await _userCredContext.Users.FindAsync(id);
            bool hasChanged = false;
            if (oldUser == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Konto nie istnieje");
            }
            oldUser.Username = newUser.Username;
            oldUser.Hash = Argon2.Hash(newUser.Password);

            if (newUser.EmployeeId != null)
            {
                if (!await EmployeeExists(newUser.EmployeeId.Value))
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Pracownik nie istnieje");
                }
                oldUser.EmployeeId = newUser.EmployeeId;
                hasChanged = true;
            }
            if (hasChanged)
            {
                oldUser.TokenVersion++;
            }
            try
            {
                await _userCredContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Nie można przypisać pracownika do drugiego konta");
            }
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> DeleteUser(short id)
        {

            var existingUser = await _userCredContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono konta");
            }
            _userCredContext.Users.Remove(existingUser);
            try
            {
                await _userCredContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Nie udało się usunąć konta");
            }
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }



        public async Task<ServiceResult> LogOutUser(short id)
        {
            var existingUser = await _userCredContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono konta");
            }
            existingUser.TokenVersion++;
            _userCredContext.SaveChanges();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> LogOutAll()
        {
            var users = await _userCredContext.Users.ToListAsync();
            if (users.Count == 0)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Brak użytkowników w systemie");
            }
            foreach (var user in users)
            {
                user.TokenVersion++;
            }
            _userCredContext.SaveChanges();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        private async Task<bool> EmployeeExists(short id)
        {
            var result = await _bikesDbContext.Employees.FindAsync(id);
            return result != null;
        }
    }
}
