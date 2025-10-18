using ams_desk_cs_backend.Data.Models;

namespace ams_desk_cs_backend.Models.Dtos;

public class FavoriteModelDto
{
    public FavoriteModelDto(Model model)
    {
        Id = model.ModelId;
        Name = model.Name;
        FrameSize = model.FrameSize;
        WheelSize = model.WheelSizeId;
        ManufacturerName = model.Manufacturer!.Name;
        ProductCode = model.ProductCode;
        PrimaryColor = model.PrimaryColor;
        SecondaryColor = model.SecondaryColor;
        Count = model.Bikes.Count;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string ManufacturerName { get; set; }
    public short FrameSize { get; set; }
    public decimal WheelSize { get; set; }
    public string? ProductCode { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public int Count { get; set; }
}