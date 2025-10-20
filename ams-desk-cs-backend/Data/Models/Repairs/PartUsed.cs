using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("parts_used")]
public class PartUsed
{
    [Key]
    [Column("part_used_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("part_id")]
    public int PartId { get; set; }

    [Required]
    [Column("repair_id")]
    public int RepairId { get; set; }

    [Required]
    [Column("amount")]
    [Range(0, int.MaxValue)]
    public float Amount { get; set; }

    [Column("price")]
    public float Price { get; set; }

    [ForeignKey(nameof(PartId))]
    [InverseProperty(nameof(Part.PartsUsed))]
    public virtual Part? Part { get; set; }

    [ForeignKey(nameof(RepairId))]
    [InverseProperty(nameof(Repair.Parts))]
    public virtual Repair? Repair { get; set; }
}