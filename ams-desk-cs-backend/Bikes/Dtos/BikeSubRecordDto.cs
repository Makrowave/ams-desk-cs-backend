namespace ams_desk_cs_backend.Bikes.Dtos;

public class BikeSubRecordDto
{
    public int Id { get; set; }
    public int Place { get; set; }
    public int StatusId { get; set; }
    public int? AssembledBy { get; set; }
}