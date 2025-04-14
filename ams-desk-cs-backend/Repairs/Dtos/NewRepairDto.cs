using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Repairs.Dtos
{
    public class NewRepairDto
    {
        [Required]
        [RegularExpression(Regexes.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [RegularExpression(Regexes.PolishText)]
        [MaxLength(200)]
        public string Issue { get; set; } = null!;
        [Required]
        [RegularExpression(Regexes.PolishText)]
        [MaxLength(40)]
        public string BikeName { get; set; } = null!;
        [Required]
        public short PlaceId { get; set; }
        [Required]
        public short TakeInEmployeeId { get; set; }
    }
}
