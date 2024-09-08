using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Models;

public partial class BikesDbContext : DbContext
{
    public BikesDbContext()
    {
    }

    public BikesDbContext(DbContextOptions<BikesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bike> Bikes { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bike>(entity =>
        {
            entity.HasKey(e => e.BikeId).HasName("bikes_pkey");

            entity.ToTable("bikes");

            entity.Property(e => e.BikeId).HasColumnName("bike_id").ValueGeneratedOnAdd();
            entity.Property(e => e.InsertionDate).HasColumnName("insertion_date");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.SaleDate).HasColumnName("sale_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Model).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bikes_model_id_fkey");

            entity.HasOne(d => d.Place).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bikes_place_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bikes_status_id_fkey");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("manufacturer_pkey");

            entity.ToTable("manufacturers");

            entity.Property(e => e.ManufacturerId)
                .ValueGeneratedNever()
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufacturerName)
                .HasMaxLength(20)
                .HasColumnName("manufacturer_name");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("models_pkey");

            entity.ToTable("models");

            entity.HasIndex(e => e.EanCode, "models_ean_code_key").IsUnique();

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.EanCode)
                .HasMaxLength(13)
                .HasColumnName("ean_code");
            entity.Property(e => e.ModelName)
                .HasMaxLength(50)
                .HasColumnName("model_name");
            entity.Property(e => e.FrameSize).HasColumnName("frame_size");
            entity.Property(e => e.IsElectric).HasColumnName("is_electric");
            entity.Property(e => e.IsWoman).HasColumnName("is_woman");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .HasColumnName("product_code");
            entity.Property(e => e.WheelSize).HasColumnName("wheel_size");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Models)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("models_manufacturer_id_fkey");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.PlaceId).HasName("places_pkey");

            entity.ToTable("places");

            entity.Property(e => e.PlaceId)
                .ValueGeneratedNever()
                .HasColumnName("place_id");
            entity.Property(e => e.PlaceName)
                .HasMaxLength(16)
                .HasColumnName("place_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("statuses_pkey");

            entity.ToTable("statuses");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(16)
                .HasColumnName("status_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
