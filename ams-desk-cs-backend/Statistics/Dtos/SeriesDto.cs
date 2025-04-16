namespace ams_desk_cs_backend.Statistics.Dtos;

public class SeriesDto<T>
{
    public string Label { get; set; } = null!;
    public List<T> Data { get; set; } = [];
}

