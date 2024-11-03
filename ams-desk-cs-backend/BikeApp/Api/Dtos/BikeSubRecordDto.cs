namespace ams_desk_cs_backend.BikeApp.Api.Dtos
{
    public class BikeSubRecordDto
    {
        public int Id { get; set; }
        public required string Place { get; set; }
        public int StatusId { get; set; }
        public required string Status { get; set; }
        public string? AssembledBy { get; set; }
    }
}
