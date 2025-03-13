using ams_desk_cs_backend.BikeApp.Data.Models;
using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Data;

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
    //Repairs
    public virtual DbSet<Part> Parts { get; set; }
    public virtual DbSet<PartCategory> PartCategories { get; set; }
    public virtual DbSet<PartUsed> PartsUsed { get; set; }
    public virtual DbSet<Repair> Repairs { get; set; }
    public virtual DbSet<RepairStatus> RepairStatuses { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
    public virtual DbSet<ServiceDone> ServicesDone { get; set; }
    public virtual DbSet<Unit> Units { get; set; }

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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bikes_model_id_fkey");

            entity.HasOne(d => d.Place).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bikes_place_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bikes_status_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Bikes)
                .HasForeignKey(d => d.AssembledBy)
                .OnDelete(DeleteBehavior.Restrict)
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

            //entity.HasIndex(e => e.EanCode, "models_ean_code_key").IsUnique();

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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("models_manufacturer_id_fkey");
            entity.HasOne(d => d.Category).WithMany(p => p.Models)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("models_category_id_fkey");
            entity.HasOne(d => d.Color).WithMany(p => p.Models)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("models_color_id_fkey");
            entity.HasOne(d => d.WheelSize).WithMany(p => p.Models)
                .HasForeignKey(d => d.WheelSizeId)
                .OnDelete(DeleteBehavior.Restrict)
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

            entity.HasIndex(e => e.StatusName).IsUnique();
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

            entity.HasIndex(e => e.ColorName).IsUnique();
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

            entity.HasIndex(e => e.EmployeeName).IsUnique();
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

            entity.HasIndex(e => e.CategoryName).IsUnique();
        });
        modelBuilder.Entity<WheelSize>(entity =>
        {
            entity.HasKey(e => e.WheelSizeId).HasName("wheel_size_pkey");
            entity.ToTable("wheel_sizes");
            entity.Property(e => e.WheelSizeId)
                .HasColumnName("wheel_size")
                .ValueGeneratedNever();
        });
        //Repairs
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("part_pkey");
            entity.ToTable("parts");
            entity.Property(e => e.PartId)
                .HasColumnName("part_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.PartName)
                .HasColumnName("part_name")
                .ValueGeneratedOnAdd()
                .HasMaxLength(40);
            entity.Property(e => e.PartCategoryId)
                .HasColumnName("part_category_id");
            entity.Property(e => e.Price)
                .HasColumnName("part_price");
            entity.Property(e => e.UnitId)
                .HasColumnName("unit_id");

            entity.HasOne(d => d.PartCategory).WithMany(p => p.Parts)
                .HasForeignKey(d => d.PartCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("part_category_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Parts)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("part_unit_fkey");

            //Here I don't want index since parts won't be standardized ATM
        });
        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.PartCategoryId).HasName("part_category_pkey");
            entity.ToTable("part_categories");
            entity.Property(e => e.PartCategoryId)
                .HasColumnName("part_category_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.PartCategoryName)
                .HasColumnName("part_name")
                .ValueGeneratedOnAdd()
                .HasMaxLength(30);

            entity.HasIndex(e => e.PartCategoryName).IsUnique();
        });
        modelBuilder.Entity<PartUsed>(entity =>
        {
            entity.HasKey(e => e.PartUsedId).HasName("part_used_pkey");
            entity.ToTable("parts_used");
            entity.Property(e => e.PartUsedId)
                .HasColumnName("part_used_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.PartId)
                .HasColumnName("part_id");
            entity.Property(e => e.RepairId)
                .HasColumnName("repair_id");
            entity.Property(e => e.Amount)
                .HasColumnName("amount");
            entity.Property(e => e.Price)
                .HasColumnName("price");
            entity.HasOne(d => d.Part).WithMany(p => p.PartsUsed)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("part_fkey");
            entity.HasOne(d => d.Repair).WithMany(p => p.Parts)
                .HasForeignKey(d => d.RepairId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("repair_part_used_fkey");
        });


        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("service_pkey");
            entity.ToTable("services");
            entity.Property(e => e.ServiceId)
                .HasColumnName("service_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Price)
                .HasColumnName("price");
            entity.Property(e => e.ServiceName)
                .HasColumnName("service_name")
                .ValueGeneratedOnAdd()
                .HasMaxLength(40);
            entity.Property(e => e.ServiceCategoryId)
                .HasColumnName("service_category_id");

            entity.HasOne(d => d.ServiceCategory).WithMany(p => p.Services)
                .HasForeignKey(d => d.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("service_category_fkey");
            entity.HasIndex(e => e.ServiceName).IsUnique();
        });
        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.ServiceCategoryId).HasName("service_category_pkey");
            entity.ToTable("service_categories");
            entity.Property(e => e.ServiceCategoryId)
                .HasColumnName("service_category_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ServiceCategoryName)
                .HasColumnName("service_name")
                .ValueGeneratedOnAdd()
                .HasMaxLength(30);
            entity.HasIndex(e => e.ServiceCategoryName).IsUnique();
        });
        modelBuilder.Entity<ServiceDone>(entity =>
        {
            entity.HasKey(e => e.ServiceDoneId).HasName("service_used_pkey");
            entity.ToTable("services_done");
            entity.Property(e => e.ServiceDoneId)
                .HasColumnName("service_done_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ServiceId)
                .HasColumnName("service_id");
            entity.Property(e => e.RepairId)
                .HasColumnName("repair_id");
            entity.Property(e => e.Price)
                .HasColumnName("price");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicesDone)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("service_fkey");
            entity.HasOne(d => d.Repair).WithMany(p => p.Services)
                .HasForeignKey(d => d.RepairId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("repair_service_done_fkey");
        });

        modelBuilder.Entity<RepairStatus>(entity =>
        {
            entity.HasKey(e => e.RepairStatusId).HasName("repair_status_pkey");
            entity.ToTable("repair_statuses");
            entity.Property(e => e.RepairStatusId)
                .HasColumnName("repair_status_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasColumnName("repair_status_name")
                .HasMaxLength(30);
            entity.Property(e => e.Color)
                .HasColumnName("color")
                .HasMaxLength(7);
            entity.HasIndex(e => e.Name).IsUnique();
        });


        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => e.RepairId).HasName("repair_pkey");
            entity.ToTable("repairs");
            entity.Property(e => e.RepairId)
                .HasColumnName("repair_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(9);
            entity.Property(e => e.BikeName)
                .HasColumnName("bike_name")
                .HasMaxLength(40);
            entity.Property(e => e.Issue)
                .HasColumnName("issue")
                .HasMaxLength(200);
            entity.Property(e => e.ArrivalDate)
                .HasColumnName("arrival_date");
            entity.Property(e => e.CollectionDate)
                .HasColumnName("collection_date");
            entity.Property(e => e.RepairEmployeeId)
                .HasColumnName("repair_employee_id");
            entity.Property(e => e.CollectionEmployeeId)
                .HasColumnName("collection_employee_id");
            entity.Property(e => e.Discount)
                .HasColumnName("discount");
            entity.Property(e => e.StatusId)
                .HasColumnName("status_id");
            entity.Property(e => e.PlaceId)
                .HasColumnName("place_id");
            entity.Property(e => e.Note)
                .HasColumnName("note")
                .HasMaxLength(1000);

            entity.HasOne(d => d.CollectionEmployee).WithMany(p => p.CollectionRepairs)
                .HasForeignKey(d => d.CollectionEmployeeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("collection_employee_fkey");

            entity.HasOne(d => d.RepairEmployee).WithMany(p => p.RepairRepairs)
                .HasForeignKey(d => d.RepairEmployeeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("repair_employee_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("repair_status_fkey");

            entity.HasOne(d => d.Place).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("repair_places_fkey");

        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("unit");
            entity.ToTable("units");
            entity.Property(e => e.UnitId)
                .HasColumnName("unit_id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.UnitName)
                .HasColumnName("unit_name");
        });

            modelBuilder.Entity<RepairStatus>().HasData([
            new RepairStatus {RepairStatusId = 1, Color = "#FFA500", Name = "Przyjęto"},
            new RepairStatus {RepairStatusId = 2, Color = "#27a8be", Name = "Reklamacja"},
            new RepairStatus {RepairStatusId = 3, Color = "#fff0c2", Name = "W trakcie"},
            new RepairStatus {RepairStatusId = 4, Color = "#a6e1f7", Name = "Oczekuje na części"},
            new RepairStatus {RepairStatusId = 5, Color = "#c8e6c9", Name = "Zakończono"},
            new RepairStatus {RepairStatusId = 6, Color = "#82e085", Name = "Powiadomiono"},
            new RepairStatus {RepairStatusId = 7, Color = "#32fc39", Name = "Wydano"},
            new RepairStatus {RepairStatusId = 8, Color = "#54afff", Name = "Kontakt z klientem"},
            ]);

        modelBuilder.Entity<Unit>().HasData([
            new Unit {UnitId = 1, IsDiscrete = true, UnitName = "szt."},
            new Unit {UnitId = 2, IsDiscrete = false, UnitName = "m"},
            new Unit {UnitId = 3, IsDiscrete = false, UnitName = "cm"},
            ]);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
