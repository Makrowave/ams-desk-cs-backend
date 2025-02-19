using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;

namespace ams_desk_cs_backend.BikeApp.Dtos.Repairs
{
    public class ShortRepairDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string BikeName { get; set; } = null!;
        public RepairStatus Status { get; set; } = null!;
        public DateOnly Date { get; set; }
        public short PlaceId { get; set; }
        public string PlaceName { get; set; } = null!;

    }
}
