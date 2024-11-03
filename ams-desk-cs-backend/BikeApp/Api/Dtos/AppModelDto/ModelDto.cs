namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

public class ModelDto
{
    public int? ModelId { get; set; }

    public string? ProductCode { get; set; }

    public string? EanCode { get; set; }
    public string? ModelName { get; set; }
    public short? FrameSize { get; set; }
    public short? WheelSize { get; set; }
    public bool? IsWoman { get; set; }

    public short? ManufacturerId { get; set; }
    public short? ColorId { get; set; }
    public short? CategoryId { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }

    public int? Price { get; set; }

    public bool? IsElectric { get; set; }
    public string? Link { get; set; }
    public DateOnly? InsertionDate { get; set; }
}
