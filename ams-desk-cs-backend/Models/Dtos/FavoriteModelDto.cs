using ams_desk_cs_backend.Data.Models;

namespace ams_desk_cs_backend.Models.Dtos;

public class FavoriteModelDto
{
    public FavoriteModelDto(Model model)
    {
        ModelId = model.ModelId;
        ModelName = model.ModelName;
        FrameSize = model.FrameSize;
        WheelSize = model.WheelSizeId;
        ManufacturerName = model.Manufacturer!.Name;
        ProductCode = model.ProductCode;
        PrimaryColor = model.PrimaryColor;
        SecondaryColor = model.SecondaryColor;
        Count = model.Bikes.Count;
    }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
    public string ManufacturerName { get; set; }
    public short FrameSize { get; set; }
    public decimal WheelSize { get; set; }
    public string? ProductCode { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public int Count { get; set; }
}