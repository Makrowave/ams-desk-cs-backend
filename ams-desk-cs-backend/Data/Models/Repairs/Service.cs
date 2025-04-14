using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Data.Models.Repairs;

public class Service
{
    [Required]
    public short ServiceId { get; set; }
    [Required]
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(40)]
    public string ServiceName { get; set; } = null!;
    [Required]
    [Range(0, float.MaxValue)]
    public float Price { get; set; }
    [Required]
    public short ServiceCategoryId { get; set; }
    public virtual ServiceCategory? ServiceCategory { get; set; }
    public virtual ICollection<ServiceDone> ServicesDone { get; set; } = [];
}