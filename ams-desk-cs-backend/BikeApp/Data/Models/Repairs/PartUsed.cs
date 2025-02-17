using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
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
        public Part? Part { get; set; }
        public Repair? Repair { get; set; }
    }
}
