namespace ams_desk_cs_backend.BikeFilters.Dtos;

public class WheelSizeDto
{

    public WheelSizeDto(decimal id)
    {
        Id = id;
        Name = $"{id:N1}";
    }
    public decimal Id { get; set; }
    public string Name { get; set; }
}