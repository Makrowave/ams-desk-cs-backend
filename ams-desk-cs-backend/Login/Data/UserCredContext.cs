using ams_desk_cs_backend.Login.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Login.Data;

public partial class UserCredContext : DbContext
{
    public UserCredContext()
    {
    }

    public UserCredContext(DbContextOptions<UserCredContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Hash)
                .HasMaxLength(120)
                .HasColumnName("hash");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");
            entity.Property(e => e.TokenVersion)
                .HasColumnName("token_version");
            entity.Property(e => e.IsAdmin)
                .HasColumnName("is_admin");
            entity.Property(e => e.AdminHash)
                .HasMaxLength(120)
                .HasColumnName("admin_hash");
            entity.Property(e => e.EmployeeId)
                .HasColumnName("employee_id");
        });

        const string defaultPass = "administrator";
        var admin = new User("admin", defaultPass)
        {
            UserId = -1,
            IsAdmin = true,
        };
        admin.SetAdminPassword(defaultPass);
        modelBuilder.Entity<User>().HasData([
            admin
        ]);
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
