using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ams_desk_cs_backend.Shared;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("parts")]
public class Part
{
    [Key]
    [Column("part_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PartId { get; set; }

    [Required]
    [Column("part_name")]
    [MaxLength(50)]
    [MinLength(1)]
    [RegularExpression(Regexes.PolishText)]
    public string PartName { get; set; } = null!;

    [Required]
    [Column("part_price")]
    [Range(0, float.MaxValue)]
    public float Price { get; set; }

    [Required]
    [Column("part_category_id")]
    public short PartTypeId { get; set; }

    [Required]
    [Column("unit_id")]
    public short UnitId { get; set; }

    public virtual ICollection<PartUsed> PartsUsed { get; set; } = new List<PartUsed>();

    [ForeignKey(nameof(UnitId))]
    [InverseProperty(nameof(Unit.Parts))]
    public virtual Unit? Unit { get; set; }

    [ForeignKey(nameof(PartTypeId))]
    [InverseProperty(nameof(PartType.Parts))]
    public virtual PartType? PartType { get; set; }
}