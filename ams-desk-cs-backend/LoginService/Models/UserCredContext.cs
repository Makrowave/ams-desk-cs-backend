using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.LoginService.Models;

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

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Hash)
                .HasMaxLength(64)
                .HasColumnName("hash");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
