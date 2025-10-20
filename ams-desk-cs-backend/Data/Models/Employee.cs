using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.Data.Models.Repairs;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("employees")]
public partial class Employee
{
    [Key]
    [Column("employee_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short iD { get; set; }

    [Column("employee_name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Column("employees_order")]
    public required short Order { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();

    [InverseProperty(nameof(Repair.RepairEmployee))]
    public virtual ICollection<Repair> RepairRepairs { get; set; } = new List<Repair>();

    [InverseProperty(nameof(Repair.CollectionEmployee))]
    public virtual ICollection<Repair> CollectionRepairs { get; set; } = new List<Repair>();

    [InverseProperty(nameof(Repair.TakeInEmployee))]
    public virtual ICollection<Repair> TakeInRepairs { get; set; } = new List<Repair>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}