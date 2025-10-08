using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Repairs;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data;

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
    public virtual DbSet<PartType> PartTypes { get; set; }
    public virtual DbSet<PartCategory> PartCategories { get; set; }
    public virtual DbSet<PartUsed> PartsUsed { get; set; }
    public virtual DbSet<Repair> Repairs { get; set; }
    public virtual DbSet<RepairStatus> RepairStatuses { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
    public virtual DbSet<ServiceDone> ServicesDone { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        modelBuilder.Entity<RepairStatus>().HasData([
            new RepairStatus {RepairStatusId = 1, Color = "#FFA500", Name = "Przyjęto"},
            new RepairStatus {RepairStatusId = 2, Color = "#27a8be", Name = "Reklamacja"},
            new RepairStatus {RepairStatusId = 3, Color = "#fff0c2", Name = "W trakcie"},
            new RepairStatus {RepairStatusId = 4, Color = "#a6e1f7", Name = "Oczekuje na części"},
            new RepairStatus {RepairStatusId = 5, Color = "#c8e6c9", Name = "Zakończono"},
            new RepairStatus {RepairStatusId = 6, Color = "#82e085", Name = "Powiadomiono"},
            new RepairStatus {RepairStatusId = 7, Color = "#32fc39", Name = "Wydano"},
            new RepairStatus {RepairStatusId = 8, Color = "#54afff", Name = "Kontakt z klientem"},
            new RepairStatus {RepairStatusId = 9, Color = "#f51b47", Name = "Anulowano"},
        ]);

        modelBuilder.Entity<Unit>().HasData([
            new Unit {UnitId = 1, IsDiscrete = true, UnitName = "szt."},
            new Unit {UnitId = 2, IsDiscrete = false, UnitName = "m"},
            new Unit {UnitId = 3, IsDiscrete = false, UnitName = "cm"},
            new Unit {UnitId = 4, IsDiscrete = false, UnitName = "ml"},
        ]);

        
        modelBuilder.Entity<Status>().HasData([
            new Status { StatusId = 1, StatusName = "Niezłożony", HexCode = "#fff0c2", StatusesOrder = 1 },
            new Status { StatusId = 2, StatusName = "Złożony", HexCode = "#c8e6c9", StatusesOrder = 2 },
            new Status { StatusId = 3, StatusName = "Sprzedany", HexCode = "#ff00ff", StatusesOrder = 3 },
            new Status { StatusId = 4, StatusName = "Do wysyłki", HexCode = "#f7cca6", StatusesOrder = 4 },
            new Status { StatusId = 5, StatusName = "Zadatek", HexCode = "#a6e1f7", StatusesOrder = 5 },
            new Status { StatusId = 6, StatusName = "Reklamacja", HexCode = "#27a8be", StatusesOrder = 6 }
        ]);

        modelBuilder.Entity<ServiceCategory>().HasData([
            new ServiceCategory() {ServiceCategoryId = 1, ServiceCategoryName = "Przeglądy"},
            new ServiceCategory() {ServiceCategoryId = 2, ServiceCategoryName = "Zawieszenie"},
            new ServiceCategory() {ServiceCategoryId = 3, ServiceCategoryName = "Przerzutki"},
            new ServiceCategory() {ServiceCategoryId = 4, ServiceCategoryName = "Korby"},
            new ServiceCategory() {ServiceCategoryId = 5, ServiceCategoryName = "Hamulce V-BRAKE"},
            new ServiceCategory() {ServiceCategoryId = 6, ServiceCategoryName = "Hamulce tarczowe"},
            new ServiceCategory() {ServiceCategoryId = 7, ServiceCategoryName = "Koła"},
            new ServiceCategory() {ServiceCategoryId = 8, ServiceCategoryName = "Ogumienie"},
            new ServiceCategory() {ServiceCategoryId = 9, ServiceCategoryName = "Piasty"},
            new ServiceCategory() {ServiceCategoryId = 10, ServiceCategoryName = "Stery"},
            new ServiceCategory() {ServiceCategoryId = 11, ServiceCategoryName = "Kierownica"},
            new ServiceCategory() {ServiceCategoryId = 12, ServiceCategoryName = "Siodełko"},
            new ServiceCategory() {ServiceCategoryId = 13, ServiceCategoryName = "Akcesoria"},
            new ServiceCategory() {ServiceCategoryId = 14, ServiceCategoryName = "Rower i inne"},
        ]);

        modelBuilder.Entity<Color>().HasData([
            new Color() {ColorId = 1, ColorName = "Biały", HexCode = "#ffffff", ColorsOrder = 1},
            new Color() {ColorId = 2, ColorName = "Szary", HexCode = "#7b8285", ColorsOrder = 2},
            new Color() {ColorId = 3, ColorName = "Czarny", HexCode = "#000000", ColorsOrder = 3},
            new Color() {ColorId = 4, ColorName = "Różowy", HexCode = "#f542ef", ColorsOrder = 4},
            new Color() {ColorId = 5, ColorName = "Czerwony", HexCode = "#f54242", ColorsOrder = 5},
            new Color() {ColorId = 6, ColorName = "Pomarańczowy", HexCode = "#f57d1b", ColorsOrder = 6},
            new Color() {ColorId = 7, ColorName = "Żółty", HexCode = "#f5d41b", ColorsOrder = 7},
            new Color() {ColorId = 8, ColorName = "Zielony", HexCode = "#23ba26", ColorsOrder = 8},
            new Color() {ColorId = 9, ColorName = "Turkusowy", HexCode = "#18cca8", ColorsOrder = 9},
            new Color() {ColorId = 10, ColorName = "Błękitny", HexCode = "#63e8eb", ColorsOrder = 10},
            new Color() {ColorId = 11, ColorName = "Niebieski", HexCode = "#0f5edb", ColorsOrder = 11},
            new Color() {ColorId = 12, ColorName = "Fioletowy", HexCode = "#a01ff0", ColorsOrder = 12},
            new Color() {ColorId = 13, ColorName = "Brązowy", HexCode = "#9f5228", ColorsOrder = 13}
        ]);

        modelBuilder.Entity<Category>().HasData([
            new Category() {CategoryId = 1, CategoryName = "Górski", CategoriesOrder = 1},
            new Category() {CategoryId = 2, CategoryName = "Gravel", CategoriesOrder = 2},
            new Category() {CategoryId = 3, CategoryName = "Szosowy", CategoriesOrder = 3},
            new Category() {CategoryId = 4, CategoryName = "Cross", CategoriesOrder = 4},
            new Category() {CategoryId = 5, CategoryName = "Trekking", CategoriesOrder = 5},
            new Category() {CategoryId = 6, CategoryName = "Miejski", CategoriesOrder = 6},
            new Category() {CategoryId = 7, CategoryName = "Dirt", CategoriesOrder = 7},
            new Category() {CategoryId = 8, CategoryName = "Bmx", CategoriesOrder = 8},
            new Category() {CategoryId = 9, CategoryName = "Dziecięcy", CategoriesOrder = 9},
            new Category() {CategoryId = 10, CategoryName = "Składak", CategoriesOrder = 10}
        ]);

        modelBuilder.Entity<WheelSize>().HasData([
            new WheelSize {WheelSizeId = 12},
            new WheelSize {WheelSizeId = 16},
            new WheelSize {WheelSizeId = 20},
            new WheelSize {WheelSizeId = 24},
            new WheelSize {WheelSizeId = 26},
            new WheelSize {WheelSizeId = 27},
            new WheelSize {WheelSizeId = 28},
            new WheelSize {WheelSizeId = 29},
        ]);
        
        const string defaultPass = "administrator";
        var admin = new User("admin", defaultPass)
        {
            Username = "admin",
            UserId = -1,
            IsAdmin = true,
        };
        admin.SetAdminPassword(defaultPass);
        modelBuilder.Entity<User>().HasData([
            admin
        ]);

        // modelBuilder.Entity<RepairStatus>().HasData([
        //     new RepairStatus {RepairStatusId = 9, Color = "#f51b47", Name = "Anulowano"},
        // ]);
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
