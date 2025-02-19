using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.Repairs
{
    public class RepairDto
    {
        [Required]
        public int RepairId { get; set; }
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string BikeName { get; set; } = null!;
        [Required]
        [RegularExpression(Regexes.PolishText)]
        [MaxLength(200)]
        public string Issue { get; set; } = null!;
        [Required]
        public DateOnly ArrivalDate { get; set; }
        public DateOnly? CollectionDate { get; set; }
        public short? RepairEmployeeId { get; set; }
        public short? CollectionEmployeeId { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float Discount { get; set; } = 0;
        [Required]
        [Range(0, float.MaxValue)]
        public float AdditionalCosts { get; set; } = 0;
        [Required]
        public short StatusId { get; set; }
        [Required]
        public short PlaceId { get; set; }
        [RegularExpression(Regexes.PolishText)]
        [MaxLength(400)]
        public string? Note { get; set; }
        public RepairStatus? Status { get; set; }
        //Names without validation because they only come from backend - they don't return
        public string? RepairEmployeeName { get; set; }
        public string? CollectionEmployeeName { get; set; }
        public virtual ICollection<ServiceDone> Services { get; set; } = [];
        public virtual ICollection<PartUsed> Parts { get; set; } = [];
    }
}
