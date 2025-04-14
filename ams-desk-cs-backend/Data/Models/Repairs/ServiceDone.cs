using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Data.Models.Repairs;

public class ServiceDone
{
    [Required]
    public int ServiceDoneId { get; set; }
    [Required]
    public short ServiceId { get; set; }
    [Required]
    public int RepairId { get; set; }
    // Price it was sold for - won't change if Part's price changes
    public float Price { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Repair? Repair { get; set; }
}