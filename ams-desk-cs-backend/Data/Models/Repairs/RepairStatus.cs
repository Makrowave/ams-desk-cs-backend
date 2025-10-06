using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Index(nameof(Name), IsUnique = true)]
[Table("repair_statuses")]
public class RepairStatus
{
    [Key]
    [Column("repair_status_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short RepairStatusId { get; set; }

    [Column("repair_status_name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Column("color")]
    [MaxLength(7)]
    public required string Color { get; set; }

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}