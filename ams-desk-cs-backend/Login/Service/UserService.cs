﻿using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Login.Data;
using ams_desk_cs_backend.Login.Data.Models;
using ams_desk_cs_backend.Login.Dto;
using ams_desk_cs_backend.Login.Interface;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Login.Service;

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

    public async Task<ServiceResult> PostUser(UserDto userDto)
    {
        if (userDto.Password == null || userDto.EmployeeId == null)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Brak hasła lub użytkownika");
        }
        var user = new User(userDto.Username, userDto.Password, userDto.EmployeeId.Value);
        _userCredContext.Add(user);
        try
        {
            await _userCredContext.SaveChangesAsync();
        }
        catch (Exception)
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
        if (newUser.Password != null)
        {
            oldUser.SetPassword(newUser.Password);
        }
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
        catch (Exception)
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
        catch (Exception)
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