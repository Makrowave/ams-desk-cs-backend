using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;
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

    public virtual DbSet<ModelColor> Colors { get; set; }
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
    public virtual DbSet<Delivery> Deliveries { get; set; }
    public virtual DbSet<DeliveryDocument> DeliveryDocuments { get; set; }
    public virtual DbSet<DeliveryItem> DeliveryItems { get; set; }
    public virtual DbSet<TemporaryModel> TemporaryModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryItem>(entity =>
            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_DeliveredItem_OneItemRef",
                    "(\"model_id\" IS NOT NULL AND \"temporary_model_id\" IS NULL) OR (\"ItemId\" IS NULL AND \"TemporaryItemId\" IS NOT NULL)");
            })
        );

        modelBuilder.Entity<RepairStatus>().HasData([
            new RepairStatus {Id = 1, Color = "#FFA500", Name = "Przyjęto"},
            new RepairStatus {Id = 2, Color = "#27a8be", Name = "Reklamacja"},
            new RepairStatus {Id = 3, Color = "#fff0c2", Name = "W trakcie"},
            new RepairStatus {Id = 4, Color = "#a6e1f7", Name = "Oczekuje na części"},
            new RepairStatus {Id = 5, Color = "#c8e6c9", Name = "Zakończono"},
            new RepairStatus {Id = 6, Color = "#82e085", Name = "Powiadomiono"},
            new RepairStatus {Id = 7, Color = "#32fc39", Name = "Wydano"},
            new RepairStatus {Id = 8, Color = "#54afff", Name = "Kontakt z klientem"},
            new RepairStatus {Id = 9, Color = "#f51b47", Name = "Anulowano"},
        ]);

        modelBuilder.Entity<Unit>().HasData([
            new Unit {Id = 1, IsDiscrete = true, Name = "szt."},
            new Unit {Id = 2, IsDiscrete = false, Name = "m"},
            new Unit {Id = 3, IsDiscrete = false, Name = "cm"},
            new Unit {Id = 4, IsDiscrete = false, Name = "ml"},
        ]);

        
        modelBuilder.Entity<Status>().HasData([
            new Status { Id = 1, Name = "Niezłożony", Color = "#fff0c2", Order = 1 },
            new Status { Id = 2, Name = "Złożony", Color = "#c8e6c9", Order = 2 },
            new Status { Id = 3, Name = "Sprzedany", Color = "#ff00ff", Order = 3 },
            new Status { Id = 4, Name = "Do wysyłki", Color = "#f7cca6", Order = 4 },
            new Status { Id = 5, Name = "Zadatek", Color = "#a6e1f7", Order = 5 },
            new Status { Id = 6, Name = "Reklamacja", Color = "#27a8be", Order = 6 }
        ]);

        modelBuilder.Entity<ServiceCategory>().HasData([
            new ServiceCategory() {Id = 1, Name = "Przeglądy"},
            new ServiceCategory() {Id = 2, Name = "Zawieszenie"},
            new ServiceCategory() {Id = 3, Name = "Przerzutki"},
            new ServiceCategory() {Id = 4, Name = "Korby"},
            new ServiceCategory() {Id = 5, Name = "Hamulce V-BRAKE"},
            new ServiceCategory() {Id = 6, Name = "Hamulce tarczowe"},
            new ServiceCategory() {Id = 7, Name = "Koła"},
            new ServiceCategory() {Id = 8, Name = "Ogumienie"},
            new ServiceCategory() {Id = 9, Name = "Piasty"},
            new ServiceCategory() {Id = 10, Name = "Stery"},
            new ServiceCategory() {Id = 11, Name = "Kierownica"},
            new ServiceCategory() {Id = 12, Name = "Siodełko"},
            new ServiceCategory() {Id = 13, Name = "Akcesoria"},
            new ServiceCategory() {Id = 14, Name = "Rower i inne"},
        ]);

        modelBuilder.Entity<ModelColor>().HasData([
            new ModelColor() {Id = 1, Name = "Biały", Color = "#ffffff", Order = 1},
            new ModelColor() {Id = 2, Name = "Szary", Color = "#7b8285", Order = 2},
            new ModelColor() {Id = 3, Name = "Czarny", Color = "#000000", Order = 3},
            new ModelColor() {Id = 4, Name = "Różowy", Color = "#f542ef", Order = 4},
            new ModelColor() {Id = 5, Name = "Czerwony", Color = "#f54242", Order = 5},
            new ModelColor() {Id = 6, Name = "Pomarańczowy", Color = "#f57d1b", Order = 6},
            new ModelColor() {Id = 7, Name = "Żółty", Color = "#f5d41b", Order = 7},
            new ModelColor() {Id = 8, Name = "Zielony", Color = "#23ba26", Order = 8},
            new ModelColor() {Id = 9, Name = "Turkusowy", Color = "#18cca8", Order = 9},
            new ModelColor() {Id = 10, Name = "Błękitny", Color = "#63e8eb", Order = 10},
            new ModelColor() {Id = 11, Name = "Niebieski", Color = "#0f5edb", Order = 11},
            new ModelColor() {Id = 12, Name = "Fioletowy", Color = "#a01ff0", Order = 12},
            new ModelColor() {Id = 13, Name = "Brązowy", Color = "#9f5228", Order = 13}
        ]);

        modelBuilder.Entity<Category>().HasData([
            new Category() {Id = 1, Name = "Górski", Order = 1},
            new Category() {Id = 2, Name = "Gravel", Order = 2},
            new Category() {Id = 3, Name = "Szosowy", Order = 3},
            new Category() {Id = 4, Name = "Cross", Order = 4},
            new Category() {Id = 5, Name = "Trekking", Order = 5},
            new Category() {Id = 6, Name = "Miejski", Order = 6},
            new Category() {Id = 7, Name = "Dirt", Order = 7},
            new Category() {Id = 8, Name = "Bmx", Order = 8},
            new Category() {Id = 9, Name = "Dziecięcy", Order = 9},
            new Category() {Id = 10, Name = "Składak", Order = 10}
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
            Id = -1,
            IsAdmin = true,
        };
        admin.SetAdminPassword(defaultPass);
        modelBuilder.Entity<User>().HasData([
            admin
        ]);

        // modelBuilder.Entity<RepairStatus>().HasData([
        //     new RepairStatus {Id = 9, Color = "#f51b47", Name = "Anulowano"},
        // ]);
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
