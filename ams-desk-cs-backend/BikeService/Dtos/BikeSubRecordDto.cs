namespace ams_desk_cs_backend.BikeService.Dtos
{
    public class BikeSubRecordDto
    {
        public int Id { get; set; }
        public string Place { get; set; } = null!;
        public int StatusId { get; set; }
        public string Status { get; set; } = null!;
    }
}
