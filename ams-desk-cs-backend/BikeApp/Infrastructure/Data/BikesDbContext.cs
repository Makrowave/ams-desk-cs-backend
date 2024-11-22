using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data;

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

    public virtual DbSet<Color> Colors { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<WheelSize> WheelSizes { get; set; }

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
            entity.Property(e => e.SalePrice).HasColumnName("sale_price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.AssembledBy).HasColumnName("assembled_by");

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
            entity.HasOne(d => d.Employee).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.AssembledBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bikes_assembled_by_fkey");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("manufacturer_pkey");

            entity.ToTable("manufacturers");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id").ValueGeneratedOnAdd();
            entity.Property(e => e.ManufacturerId)
                .HasColumnName("manufacturer_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ManufacturerName)
                .HasMaxLength(20)
                .HasColumnName("manufacturer_name");
            entity.Property(e => e.ManufacturersOrder)
                .HasColumnName("manufacturers_order");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("models_pkey");

            entity.ToTable("models");

            entity.HasIndex(e => e.EanCode, "models_ean_code_key").IsUnique();

            entity.Property(e => e.ModelId).HasColumnName("model_id").ValueGeneratedOnAdd();
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
            entity.Property(e => e.InsertionDate).HasColumnName("insertion_date");
            entity.Property(e => e.ColorId).HasColumnName("color_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .HasColumnName("product_code");
            entity.Property(e => e.WheelSizeId).HasColumnName("wheel_size");
            entity.Property(e => e.PrimaryColor).HasColumnType("CHAR(7)").HasColumnName("primary_color");
            entity.Property(e => e.SecondaryColor).HasColumnType("CHAR(7)").HasColumnName("secondary_color");
            entity.Property(e => e.Link)
                .HasMaxLength(100)
                .HasColumnName("link");
            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Models)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("models_manufacturer_id_fkey");
            entity.HasOne(d => d.Category).WithMany(p => p.Models)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("models_category_id_fkey");
            entity.HasOne(d => d.Color).WithMany(p => p.Models)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("models_color_id_fkey");
            entity.HasOne(d => d.WheelSize).WithMany(p => p.Models)
                .HasForeignKey(d => d.WheelSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wheel_size_fkey");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.PlaceId).HasName("places_pkey");

            entity.ToTable("places");

            entity.Property(e => e.PlaceId).HasColumnName("place_id").ValueGeneratedOnAdd();
            entity.Property(e => e.PlaceId)
                .HasColumnName("place_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.PlaceName)
                .HasMaxLength(16)
                .HasColumnName("place_name");
            entity.Property(e => e.PlacesOrder)
                .HasColumnName("places_order");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("statuses_pkey");

            entity.ToTable("statuses");

            entity.Property(e => e.StatusId)
                .HasColumnName("status_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.StatusName)
                .HasMaxLength(16)
                .HasColumnName("status_name");
            entity.Property(e => e.HexCode)
                .HasMaxLength(7)
                .HasColumnName("hex_code");
            entity.Property(e => e.StatusesOrder)
                .HasColumnName("statuses_order");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("colors_pkey");

            entity.ToTable("colors");

            entity.Property(e => e.ColorId)
                .HasColumnName("color_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ColorName)
                .HasMaxLength(30)
                .HasColumnName("color_name");
            entity.Property(e => e.HexCode)
                .HasMaxLength(7)
                .HasColumnName("hex_code");
            entity.Property(e => e.ColorsOrder)
                .HasColumnName("colors_order");
        });
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employee_pkey");
            entity.ToTable("employees");
            entity.Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(30)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeesOrder)
                .HasColumnName("employees_order");
        });
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categories_pkey");
            entity.ToTable("categories");
            entity.Property(e => e.CategoryId)
                .HasColumnName("category_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.CategoryName)
                .HasMaxLength(30)
                .HasColumnName("category_name");
            entity.Property(e => e.CategoriesOrder)
                .HasColumnName("categories_order");
        });
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<WheelSize>(entity =>
        {
            entity.HasKey(e => e.WheelSizeId).HasName("wheel_size_pkey");
            entity.ToTable("wheel_sizes");
            entity.Property(e => e.WheelSizeId)
                .HasColumnName("wheel_size")
                .ValueGeneratedNever();
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
