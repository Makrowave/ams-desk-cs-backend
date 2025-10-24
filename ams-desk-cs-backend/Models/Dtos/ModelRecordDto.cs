using System.Diagnostics.CodeAnalysis;
using ams_desk_cs_backend.Data.Models;

namespace ams_desk_cs_backend.Models.Dtos;

public class ModelRecordDto
{
    [SetsRequiredMembers]
    public ModelRecordDto(Model model, int bikeCount, IEnumerable<PlaceBikeCountDto> placeBikeCount)
    {
        Id = model.ModelId;
        ProductCode = model.ProductCode;
        EanCode = model.EanCode;
        Name = model.Name;
        FrameSize = model.FrameSize;
        WheelSize = model.WheelSizeId;
        ManufacturerId = model.ManufacturerId;
        Price = model.Price;
        IsWoman = model.IsWoman;
        IsElectric = model.IsElectric;
        PrimaryColor = model.PrimaryColor;
        SecondaryColor = model.SecondaryColor;
        CategoryId = model.CategoryId;
        ColorId = model.ColorId;
        Link = model.Link;
        Favorite = model.Favorite;
        BikeCount = bikeCount;
        PlaceBikeCount = placeBikeCount;
    }

    public int Id { get; set; }
    public string? ProductCode { get; set; }
    public string? EanCode { get; set; }
    public required string Name { get; set; }
    public short FrameSize { get; set; }
    public decimal WheelSize { get; set; }
    public short ManufacturerId { get; set; }
    public int Price { get; set; }
    public bool IsWoman { get; set; }
    public bool IsElectric { get; set; }
    public int BikeCount { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public short CategoryId { get; set; }
    public short? ColorId { get; set; }
    public string? Link { get; set; }
    public bool Favorite { get; set; }

    public required IEnumerable<PlaceBikeCountDto> PlaceBikeCount { get; set; }
}