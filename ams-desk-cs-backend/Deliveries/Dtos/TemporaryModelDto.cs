using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record TemporaryModelDto
{
    public TemporaryModelDto() {}

    public TemporaryModelDto(TemporaryModel temporaryModel)
    {
        Id = temporaryModel.Id;
        ProductCode = temporaryModel.ProductCode;
        EanCode = temporaryModel.EanCode;
        Name = temporaryModel.Name;
        FrameSize = temporaryModel.FrameSize;
        IsWoman = temporaryModel.IsWoman;
        WheelSizeId = temporaryModel.WheelSizeId;
        ManufacturerId = temporaryModel.ManufacturerId;
        ColorId = temporaryModel.ColorId;
        CategoryId = temporaryModel.CategoryId;
        Price = temporaryModel.Price;
        PrimaryColor = temporaryModel.PrimaryColor;
        SecondaryColor = temporaryModel.SecondaryColor;
        IsElectric = temporaryModel.IsElectric;
        Link = temporaryModel.Link;
    }
    
    public int Id { get; init; }
    
    [MaxLength(30)]
    public string? ProductCode { get; init; }
    
    [MaxLength(13)]
    public string? EanCode { get; init; }
    
    [MaxLength(50)]
    public string? Name { get; init; }
    
    public short? FrameSize { get; init; }
    
    public bool? IsWoman { get; init; }
    
    public decimal? WheelSizeId { get; init; }
    
    public short? ManufacturerId { get; init; }
    
    public short? ColorId { get; init; }
    
    public short? CategoryId { get; init; }
    
    public string? PrimaryColor { get; init; }
    
    public string? SecondaryColor { get; init; }
    
    public int? Price { get; init; }
    
    public bool? IsElectric { get; init; }
    
    [MaxLength(160)]
    public string? Link { get; init; }
    
    public DateOnly InsertionDate { get; init; }
}