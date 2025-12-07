namespace ams_desk_cs_backend.BikeProperties.Dtos;

public class WheelSizeDto
{

    public WheelSizeDto(decimal id)
    {
        Id = (int)id;
        Name = $"{id:N1}";
    }
    public int Id { get; set; }
    public string Name { get; set; }
}