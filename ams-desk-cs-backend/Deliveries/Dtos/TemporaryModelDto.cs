using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record TemporaryModelDto
{
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