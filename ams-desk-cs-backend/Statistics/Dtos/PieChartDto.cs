namespace ams_desk_cs_backend.Statistics.Dtos;

public class PieChartDto
{
    public int Id { get; set; }
    public String Name { get; set; } = null!;
    public float Quantity { get; set; } = 0;
    public float Value { get; set; } = 0;
}