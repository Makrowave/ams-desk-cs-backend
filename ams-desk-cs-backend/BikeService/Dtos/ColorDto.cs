namespace ams_desk_cs_backend.BikeService.Dtos
{
    public class ColorDto
    {
        public short ColorId { get; set; }
        public required string ColorName { get; set; }
        public required string HexCode { get; set; }
    }
}
