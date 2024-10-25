namespace ams_desk_cs_backend.BikeService.Dtos
{
    public class StatusDto
    {
        public short StatusId { get; set; }

        public required string StatusName { get; set; }
        public required string HexCode { get; set; }
    }
}
