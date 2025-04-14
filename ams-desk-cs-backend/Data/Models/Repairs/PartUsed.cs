using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Data.Models.Repairs;

public class PartUsed
{
    [Required]
    public int PartUsedId { get; set; }
    [Required]
    public int PartId { get; set; }
    [Required]
    public int RepairId { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public float Amount { get; set; }
    // Price it was sold for - won't change if Part's price changes
    public float Price { get; set; }
    public Part? Part { get; set; }
    public Repair? Repair { get; set; }
}