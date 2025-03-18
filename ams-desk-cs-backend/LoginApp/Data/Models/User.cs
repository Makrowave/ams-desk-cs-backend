using System;
using System.Collections.Generic;
using Isopoh.Cryptography.Argon2;

namespace ams_desk_cs_backend.LoginApp.Data.Models;

public partial class User
{
    public User(string username, string password)
    {
        Username = username;
        Hash = Argon2.Hash(password);
        TokenVersion = 1;
        IsAdmin = false;

    }
    public User(string username, string password, short employeeId) : this(username, password)
    {
        EmployeeId = employeeId;
    }
    public short UserId { get; set; }

    public string Username { get; set; }
    public string Hash { get; private set; }
    public int TokenVersion { get; set; }
    public bool IsAdmin { get; set; }
    public string? AdminHash { get; private set; }
    public short? EmployeeId { get; set; }

    public void SetPassword(string password)
    {
        Hash = Argon2.Hash(password);
    }
    public void SetAdminPassword(string password)
    {
        AdminHash = Argon2.Hash(password);
    }
    
}
