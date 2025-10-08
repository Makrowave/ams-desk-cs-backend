using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(Username), Name = "users_username_key", IsUnique = true)]
[Table("users")]
public partial class User
{
    public User() {}
    public User(string username, string password)
    {
        Username = username;
        Hash = Argon2.Hash(password);
        TokenVersion = 1;
        IsAdmin = false;
    }
    public User(string username, string password, short employeeId) : this(username, password)
    {
        Username = username;
        EmployeeId = employeeId;
    }

    [Key]
    [Column("user_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short UserId { get; set; }

    [Column("username")]
    [MaxLength(32)]
    public string Username { get; set; }

    [Column("hash")]
    [MaxLength(120)]
    public string Hash { get; private set; } = null!;

    [Column("token_version")]
    public int TokenVersion { get; set; }

    [Column("is_admin")]
    public bool IsAdmin { get; set; }

    [Column("admin_hash")]
    [MaxLength(120)]
    public string? AdminHash { get; private set; }

    [Column("employee_id")]
    public short? EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    [InverseProperty("Users")]
    public virtual Employee? Employee { get; set; }

    public void SetPassword(string password)
    {
        Hash = Argon2.Hash(password);
    }

    public void SetAdminPassword(string password)
    {
        AdminHash = Argon2.Hash(password);
    }
}