using ams_desk_cs_backend.BikeService.Models;

namespace ams_desk_cs_backend.BikeService.Dtos
{
    public class AddModelDto
    {
        public string? ProductCode { get; set; }
        public string? EanCode { get; set; }
        public required string ModelName { get; set; }
        public short FrameSize { get; set; }
        public bool IsWoman { get; set; }
        public short WheelSize { get; set; }
        public short ManufacturerId { get; set; }
        public short? ColorId { get; set; }
        public short CategoryId { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public int Price { get; set; }
        public bool IsElectric { get; set; }

    }
}
