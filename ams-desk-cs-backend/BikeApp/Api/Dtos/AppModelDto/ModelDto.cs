using ams_desk_cs_backend.Shared;
using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

public class ModelDto
{
    [Required]
    public int ModelId { get; set; }
    [RegularExpression(Regexes.ProductCode, ErrorMessage = "Niepoprawny kod produktu")]
    public string? ProductCode { get; set; }
    [RegularExpression(Regexes.EanCode, ErrorMessage = "Niepoprawny kod EAN")]
    public string? EanCode { get; set; }
    [Required]
    [RegularExpression(Regexes.ModelName, ErrorMessage = "Niepoprawna nazwa modelu")]
    public string ModelName { get; set; } = null!;
    [Required]
    [Range(0, 100, ErrorMessage = "Niepoprawny rozmiar ramy")]
    public short FrameSize { get; set; }
    [Required]
    [Range(10, 30, ErrorMessage = "Niepoprawny rozmiar koła")]
    public short WheelSize { get; set; }
    [Required]
    public bool IsWoman { get; set; }
    [Required]
    public short ManufacturerId { get; set; }
    [Required]
    public short ColorId { get; set; }
    [Required]
    public short CategoryId { get; set; }
    [Required]
    [RegularExpression(Regexes.Color, ErrorMessage = "Niepoprawny kolor główny")]
    public string PrimaryColor { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.Color, ErrorMessage = "Niepoprawny drugi kolor")]
    public string SecondaryColor { get; set; } = null!;
    [Required]
    [Range(100, 100000, ErrorMessage = "Niepoprawna cena")]
    public int Price { get; set; }
    [Required]
    public bool IsElectric { get; set; }
    [RegularExpression(Regexes.Link, ErrorMessage = "Niepoprawny link")]
    public string? Link { get; set; } = null!;
    [Required]
    public DateOnly? InsertionDate { get; set; }
}
