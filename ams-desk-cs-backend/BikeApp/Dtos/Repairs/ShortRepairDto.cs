using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.Repairs
{
    public class ShortRepairDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public short StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public DateOnly Date { get; set; }
        public short PlaceId { get; set; }
        public string PlaceName { get; set; } = null!;

    }
}
