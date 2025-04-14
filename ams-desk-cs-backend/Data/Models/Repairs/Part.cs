using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Data.Models.Repairs;

public class Part
{
    [Required]
    public int PartId { get; set; }
    [Required]
    [MaxLength(40)]
    [MinLength(1)]
    [RegularExpression(Regexes.PolishText)]
    public string PartName { get; set; } = null!;
    [Required]
    [Range(0,float.MaxValue)]
    public float Price { get; set; }
    [Required]
    public short PartTypeId { get; set; }
    [Required]
    public short UnitId { get; set; }
    public virtual ICollection<PartUsed> PartsUsed { get; set; } = [];
    public virtual Unit? Unit { get; set; }
    public virtual PartType? PartType { get; set; }
}