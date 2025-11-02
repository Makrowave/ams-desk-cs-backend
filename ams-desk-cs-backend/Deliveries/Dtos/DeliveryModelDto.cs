using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliveryModelDto
{
    public DeliveryModelDto() {}

    public DeliveryModelDto(TemporaryModel temporaryModel)
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
        PrimaryColor = temporaryModel.PrimaryColor;
        SecondaryColor = temporaryModel.SecondaryColor;
        Price = temporaryModel.Price;
        IsElectric = temporaryModel.IsElectric;
        Link = temporaryModel.Link;
        InsertionDate = temporaryModel.InsertionDate;
    }

    public DeliveryModelDto(Model model)
    {
        Id = model.Id;
        ProductCode = model.ProductCode;
        EanCode = model.EanCode;
        Name = model.Name;
        FrameSize = model.FrameSize;
        IsWoman = model.IsWoman;
        WheelSizeId = model.WheelSizeId;
        ManufacturerId = model.ManufacturerId;
        ColorId = model.ColorId;
        CategoryId = model.CategoryId;
        PrimaryColor = model.PrimaryColor;
        SecondaryColor = model.SecondaryColor;
        Price = model.Price;
        IsElectric = model.IsElectric;
        Link = model.Link;
        InsertionDate = model.InsertionDate;

        Manufacturer = model.Manufacturer;
        Color = model.Color;
        Category = model.Category;
        WheelSize = model.WheelSize;
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
    
    public string? Link { get; init; }
    
    public DateOnly InsertionDate { get; init; }

    public Manufacturer? Manufacturer { get; init; }
    
    public ModelColor? Color { get; init; }
    
    public Category? Category { get; init; }
    
    public WheelSize? WheelSize { get; init; }
}
