using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("services_done")]
public class ServiceDone
{
    [Key]
    [Column("service_done_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ServiceDoneId { get; set; }

    [Required]
    [Column("service_id")]
    public short ServiceId { get; set; }

    [Required]
    [Column("repair_id")]
    public int RepairId { get; set; }

    [Column("price")]
    public float Price { get; set; }

    [ForeignKey(nameof(ServiceId))]
    [InverseProperty(nameof(Service.ServicesDone))]
    public virtual Service? Service { get; set; }

    [ForeignKey(nameof(RepairId))]
    [InverseProperty(nameof(Repair.Services))]
    public virtual Repair? Repair { get; set; }
}